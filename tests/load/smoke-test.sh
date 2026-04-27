#!/usr/bin/env bash
# =============================================================================
# Smoke / Load Test Script for eShop Catalog API
# Runs without k6 — uses curl and basic concurrency via background jobs
#
# Usage:
#   ./tests/load/smoke-test.sh                    # defaults to localhost:5000
#   ./tests/load/smoke-test.sh http://my-api:8080 # custom base URL
#   CONCURRENT=20 REQUESTS=100 ./tests/load/smoke-test.sh  # custom load
# =============================================================================

set -euo pipefail

BASE_URL="${1:-http://localhost:5000}"
CONCURRENT="${CONCURRENT:-10}"
REQUESTS="${REQUESTS:-50}"
RESULTS_DIR="tests/load/results"

mkdir -p "$RESULTS_DIR"

echo "================================================"
echo "  eShop Catalog API — Load Test"
echo "================================================"
echo "  Base URL:    $BASE_URL"
echo "  Concurrency: $CONCURRENT"
echo "  Requests:    $REQUESTS"
echo "================================================"
echo ""

# --- Phase 1: Smoke Test ---
echo "--- Phase 1: Smoke Test ---"

check_endpoint() {
    local name="$1" url="$2" expected_status="${3:-200}"
    local status
    status=$(curl -s -o /dev/null -w "%{http_code}" --max-time 10 "$url" 2>/dev/null || echo "000")
    if [ "$status" = "$expected_status" ]; then
        echo "  PASS: $name (HTTP $status)"
        return 0
    else
        echo "  FAIL: $name (HTTP $status, expected $expected_status)"
        return 1
    fi
}

SMOKE_PASS=0
SMOKE_FAIL=0

check_endpoint "Health Check" "$BASE_URL/health" && ((SMOKE_PASS++)) || ((SMOKE_FAIL++))
check_endpoint "Catalog Items" "$BASE_URL/api/catalog/items?pageSize=5" && ((SMOKE_PASS++)) || ((SMOKE_FAIL++))
check_endpoint "Catalog Brands" "$BASE_URL/api/catalog/brands" && ((SMOKE_PASS++)) || ((SMOKE_FAIL++))
check_endpoint "Catalog Types" "$BASE_URL/api/catalog/types" && ((SMOKE_PASS++)) || ((SMOKE_FAIL++))
check_endpoint "Single Item" "$BASE_URL/api/catalog/items/1" && ((SMOKE_PASS++)) || ((SMOKE_FAIL++))
check_endpoint "Swagger JSON" "$BASE_URL/swagger/v1/swagger.json" && ((SMOKE_PASS++)) || ((SMOKE_FAIL++))

echo ""
echo "Smoke: $SMOKE_PASS passed, $SMOKE_FAIL failed"

if [ "$SMOKE_FAIL" -gt 0 ]; then
    echo "ERROR: Smoke test failed — skipping load test"
    exit 1
fi

# --- Phase 2: Response Time Baseline ---
echo ""
echo "--- Phase 2: Response Time Baseline (10 sequential requests) ---"

TOTAL_TIME=0
for i in $(seq 1 10); do
    TIME=$(curl -s -o /dev/null -w "%{time_total}" --max-time 10 "$BASE_URL/api/catalog/items?pageSize=10" 2>/dev/null || echo "10.0")
    TIME_MS=$(echo "$TIME * 1000" | bc 2>/dev/null || echo "0")
    TOTAL_TIME=$(echo "$TOTAL_TIME + $TIME_MS" | bc 2>/dev/null || echo "0")
done
AVG_TIME=$(echo "$TOTAL_TIME / 10" | bc 2>/dev/null || echo "0")
echo "  Average response time: ${AVG_TIME}ms"

# --- Phase 3: Concurrent Load Test ---
echo ""
echo "--- Phase 3: Concurrent Load Test ($CONCURRENT concurrent, $REQUESTS total) ---"

TMPDIR_LOAD=$(mktemp -d)
trap 'rm -rf "$TMPDIR_LOAD"' EXIT

ENDPOINTS=(
    "/health"
    "/api/catalog/items?pageSize=10"
    "/api/catalog/items?pageSize=5&pageIndex=1"
    "/api/catalog/brands"
    "/api/catalog/types"
    "/api/catalog/items/1"
    "/api/catalog/items/2"
    "/api/catalog/items?brand=1&type=1&pageSize=5"
)

run_request() {
    local idx="$1"
    local endpoint="${ENDPOINTS[$((idx % ${#ENDPOINTS[@]}))]}"
    local result
    result=$(curl -s -o /dev/null -w "%{http_code} %{time_total}" --max-time 15 "${BASE_URL}${endpoint}" 2>/dev/null || echo "000 15.0")
    echo "$result" > "$TMPDIR_LOAD/result_$idx.txt"
}

START_TIME=$(date +%s%N)
ACTIVE=0
for i in $(seq 1 "$REQUESTS"); do
    run_request "$i" &
    ((ACTIVE++))
    if [ "$ACTIVE" -ge "$CONCURRENT" ]; then
        wait -n 2>/dev/null || true
        ((ACTIVE--))
    fi
done
wait
END_TIME=$(date +%s%N)

ELAPSED_MS=$(( (END_TIME - START_TIME) / 1000000 ))

# Analyze results
SUCCESS=0
FAILURES=0
TOTAL_RESP_TIME=0

for f in "$TMPDIR_LOAD"/result_*.txt; do
    [ -f "$f" ] || continue
    read -r status time < "$f"
    TIME_MS=$(echo "$time * 1000" | bc 2>/dev/null || echo "0")
    TOTAL_RESP_TIME=$(echo "$TOTAL_RESP_TIME + $TIME_MS" | bc 2>/dev/null || echo "0")
    if [ "$status" = "200" ] || [ "$status" = "404" ]; then
        ((SUCCESS++))
    else
        ((FAILURES++))
    fi
done

TOTAL=$((SUCCESS + FAILURES))
AVG_RESP=$(echo "$TOTAL_RESP_TIME / $TOTAL" | bc 2>/dev/null || echo "0")
RPS=$(echo "scale=1; $TOTAL * 1000 / $ELAPSED_MS" | bc 2>/dev/null || echo "0")
ERROR_RATE=$(echo "scale=2; $FAILURES * 100 / $TOTAL" | bc 2>/dev/null || echo "0")

echo "  Total requests:     $TOTAL"
echo "  Successful:         $SUCCESS"
echo "  Failed:             $FAILURES"
echo "  Error rate:         ${ERROR_RATE}%"
echo "  Total duration:     ${ELAPSED_MS}ms"
echo "  Avg response time:  ${AVG_RESP}ms"
echo "  Requests/sec:       ${RPS}"

# --- Summary ---
echo ""
echo "================================================"
echo "  RESULTS SUMMARY"
echo "================================================"
echo "  Smoke tests:    $SMOKE_PASS/$((SMOKE_PASS + SMOKE_FAIL)) passed"
echo "  Baseline avg:   ${AVG_TIME:-0}ms (sequential)"
echo "  Load avg:       ${AVG_RESP}ms (concurrent)"
echo "  Throughput:     ${RPS} req/s"
echo "  Error rate:     ${ERROR_RATE}%"
echo "================================================"

# Save results
cat > "$RESULTS_DIR/results.json" <<EOF
{
  "timestamp": "$(date -u +%Y-%m-%dT%H:%M:%SZ)",
  "base_url": "$BASE_URL",
  "smoke_tests": { "passed": $SMOKE_PASS, "failed": $SMOKE_FAIL },
  "baseline": { "avg_response_ms": ${AVG_TIME:-0} },
  "load_test": {
    "concurrency": $CONCURRENT,
    "total_requests": $TOTAL,
    "successful": $SUCCESS,
    "failed": $FAILURES,
    "error_rate_pct": $ERROR_RATE,
    "avg_response_ms": ${AVG_RESP},
    "requests_per_sec": $RPS,
    "duration_ms": $ELAPSED_MS
  }
}
EOF

echo ""
echo "Results saved to $RESULTS_DIR/results.json"

# Exit with failure if error rate > 5%
if (( $(echo "$ERROR_RATE > 5" | bc -l 2>/dev/null || echo "0") )); then
    echo "FAIL: Error rate exceeds 5% threshold"
    exit 1
fi

echo "PASS: All performance thresholds met"
