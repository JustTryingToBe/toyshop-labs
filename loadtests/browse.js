import { browse } from './lib.js';

// Одиночный прогон — аналог Web Performance Test.
export const options = {
  vus: 1,
  iterations: 1,
  thresholds: {
    http_req_duration: ['max<2000'],
    checks: ['rate==1'],
  },
};

export default function () {
  browse();
}
