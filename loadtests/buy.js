import { buy } from './lib.js';

export const options = {
  vus: 1,
  iterations: 1,
  thresholds: {
    'group_duration{group:::06 Оформление заказа}': ['max<30000'],
    checks: ['rate==1'],
  },
};

export default function () {
  buy();
}
