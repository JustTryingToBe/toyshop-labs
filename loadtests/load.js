import { browse, buy } from './lib.js';

// Нагрузочный тест: 75% пользователей смотрят товары, 25% покупают.
// Число пользователей можно поменять без правки файла: k6 run -e BROWSERS=150 -e BUYERS=50 load.js
const BROWSERS = Number(__ENV.BROWSERS || 15);
const BUYERS = Number(__ENV.BUYERS || 5);
const HOLD = __ENV.HOLD || '1m';

export const options = {
  scenarios: {
    browsing: {
      executor: 'ramping-vus',
      exec: 'browsing',
      startVUs: 0,
      stages: [
        { duration: '30s', target: BROWSERS },
        { duration: HOLD, target: BROWSERS },
        { duration: '15s', target: 0 },
      ],
    },
    buying: {
      executor: 'ramping-vus',
      exec: 'buying',
      startVUs: 0,
      stages: [
        { duration: '30s', target: BUYERS },
        { duration: HOLD, target: BUYERS },
        { duration: '15s', target: 0 },
      ],
    },
  },
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<500'],
    checks: ['rate>0.99'],
    'group_duration{group:::06 Оформление заказа}': ['p(95)<1500'],
  },
};

export function browsing() {
  browse();
}

export function buying() {
  buy();
}
