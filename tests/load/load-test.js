// k6 Load Test — eShop Catalog API
// Run: k6 run tests/load/load-test.js
// With custom base URL: k6 run -e BASE_URL=http://localhost:5000 tests/load/load-test.js

import http from "k6/http";
import { check, sleep, group } from "k6";
import { Rate, Trend } from "k6/metrics";

const BASE_URL = __ENV.BASE_URL || "http://localhost:5000";

// Custom metrics
const errorRate = new Rate("errors");
const catalogItemsTrend = new Trend("catalog_items_duration", true);
const catalogBrandsTrend = new Trend("catalog_brands_duration", true);
const catalogTypesTrend = new Trend("catalog_types_duration", true);
const healthCheckTrend = new Trend("health_check_duration", true);

// Test scenarios
export const options = {
  scenarios: {
    // Smoke test: quick validation
    smoke: {
      executor: "constant-vus",
      vus: 1,
      duration: "10s",
      startTime: "0s",
      tags: { scenario: "smoke" },
    },
    // Load test: normal expected traffic
    load: {
      executor: "ramping-vus",
      startVUs: 0,
      stages: [
        { duration: "30s", target: 10 },  // ramp up
        { duration: "1m", target: 10 },   // hold
        { duration: "30s", target: 20 },  // ramp up more
        { duration: "1m", target: 20 },   // hold
        { duration: "30s", target: 0 },   // ramp down
      ],
      startTime: "15s",
      tags: { scenario: "load" },
    },
    // Stress test: beyond normal capacity
    stress: {
      executor: "ramping-vus",
      startVUs: 0,
      stages: [
        { duration: "30s", target: 50 },  // ramp to high load
        { duration: "1m", target: 50 },   // hold
        { duration: "30s", target: 0 },   // ramp down
      ],
      startTime: "4m",
      tags: { scenario: "stress" },
    },
  },
  thresholds: {
    http_req_duration: ["p(95)<500", "p(99)<1000"],
    errors: ["rate<0.05"],
    health_check_duration: ["p(95)<100"],
    catalog_items_duration: ["p(95)<500"],
  },
};

export default function () {
  group("Health Check", () => {
    const res = http.get(`${BASE_URL}/health`);
    healthCheckTrend.add(res.timings.duration);
    const passed = check(res, {
      "health check returns 200": (r) => r.status === 200,
      "health check responds < 100ms": (r) => r.timings.duration < 100,
    });
    errorRate.add(!passed);
  });

  group("Get Catalog Items (paginated)", () => {
    const pageIndex = Math.floor(Math.random() * 3);
    const res = http.get(
      `${BASE_URL}/api/catalog/items?pageSize=10&pageIndex=${pageIndex}`
    );
    catalogItemsTrend.add(res.timings.duration);
    const passed = check(res, {
      "catalog items returns 200": (r) => r.status === 200,
      "catalog items has data": (r) => {
        const body = JSON.parse(r.body);
        return body.data !== undefined || body.items !== undefined;
      },
    });
    errorRate.add(!passed);
  });

  group("Get Catalog Brands", () => {
    const res = http.get(`${BASE_URL}/api/catalog/brands`);
    catalogBrandsTrend.add(res.timings.duration);
    const passed = check(res, {
      "brands returns 200": (r) => r.status === 200,
    });
    errorRate.add(!passed);
  });

  group("Get Catalog Types", () => {
    const res = http.get(`${BASE_URL}/api/catalog/types`);
    catalogTypesTrend.add(res.timings.duration);
    const passed = check(res, {
      "types returns 200": (r) => r.status === 200,
    });
    errorRate.add(!passed);
  });

  group("Get Single Catalog Item", () => {
    const itemId = Math.floor(Math.random() * 10) + 1;
    const res = http.get(`${BASE_URL}/api/catalog/items/${itemId}`);
    const passed = check(res, {
      "single item returns 200 or 404": (r) =>
        r.status === 200 || r.status === 404,
    });
    errorRate.add(!passed);
  });

  group("Filter by Brand and Type", () => {
    const brandId = Math.floor(Math.random() * 5) + 1;
    const typeId = Math.floor(Math.random() * 5) + 1;
    const res = http.get(
      `${BASE_URL}/api/catalog/items?brand=${brandId}&type=${typeId}&pageSize=5`
    );
    const passed = check(res, {
      "filtered items returns 200": (r) => r.status === 200,
    });
    errorRate.add(!passed);
  });

  sleep(Math.random() * 2 + 0.5);
}

export function handleSummary(data) {
  return {
    stdout: textSummary(data, { indent: " ", enableColors: true }),
    "tests/load/results/summary.json": JSON.stringify(data, null, 2),
  };
}

// k6 built-in text summary helper
function textSummary(data, opts) {
  // k6 provides this via the built-in handleSummary
  // This is a fallback — k6 auto-generates the summary
  return JSON.stringify(
    {
      metrics: {
        http_req_duration: data.metrics.http_req_duration,
        errors: data.metrics.errors,
        iterations: data.metrics.iterations,
      },
    },
    null,
    2
  );
}
