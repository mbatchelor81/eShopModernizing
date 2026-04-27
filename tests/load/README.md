# Performance & Load Testing

## Overview

This directory contains load testing scripts for the eShop Catalog API. Two options are provided:

1. **k6 script** (`load-test.js`) — full-featured load testing with detailed metrics
2. **Shell script** (`smoke-test.sh`) — zero-dependency alternative using curl

## Baseline Performance Expectations

| Metric | Target | Notes |
|--------|--------|-------|
| Health check p95 | < 100ms | Simple liveness probe |
| Catalog items p95 | < 500ms | Paginated list with joins |
| HTTP error rate | < 5% | Under normal load |
| Concurrent users | 20+ | Without degradation |
| Requests/sec | 50+ | Single instance, basic hardware |

## Using k6

### Install k6

```bash
# macOS
brew install k6

# Ubuntu/Debian
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg \
  --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D68
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | \
  sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update && sudo apt-get install k6

# Docker
docker run --rm -i grafana/k6 run - <tests/load/load-test.js
```

### Run Tests

```bash
# Default (localhost:5000)
k6 run tests/load/load-test.js

# Custom URL
k6 run -e BASE_URL=http://my-api:8080 tests/load/load-test.js
```

### Test Scenarios

The k6 script includes three scenarios that run sequentially:

| Scenario | VUs | Duration | Purpose |
|----------|-----|----------|---------|
| Smoke | 1 | 10s | Validate endpoints are reachable |
| Load | 10→20 | 3m | Normal traffic simulation |
| Stress | 50 | 2m | Beyond normal capacity |

## Using the Shell Script

No external dependencies required — just `curl` and `bash`.

```bash
# Default settings
./tests/load/smoke-test.sh

# Custom URL
./tests/load/smoke-test.sh http://my-api:8080

# Custom concurrency and request count
CONCURRENT=20 REQUESTS=100 ./tests/load/smoke-test.sh
```

### Phases

1. **Smoke Test**: Validates all endpoints return expected status codes
2. **Baseline**: 10 sequential requests to measure single-user response time
3. **Load Test**: Concurrent requests distributed across endpoints

## Results

Results are saved to `tests/load/results/`:
- `results.json` — shell script output
- `summary.json` — k6 summary (when using k6)

## Running Against Docker Compose

```bash
# Start services
cd src/eShopModernized
docker compose up -d --build

# Wait for health
until curl -sf http://localhost:5000/health; do sleep 2; done

# Run load test
cd ../..
./tests/load/smoke-test.sh http://localhost:5000

# Or with k6
k6 run tests/load/load-test.js
```
