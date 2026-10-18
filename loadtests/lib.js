import http from 'k6/http';
import { check, group, sleep } from 'k6';

// Адрес магазина можно переопределить: k6 run -e BASE_URL=http://localhost:5080 load.js
export const BASE_URL = __ENV.BASE_URL || 'http://localhost:5080';

// ASP.NET Core кладёт в каждую форму скрытый антиподделочный токен.
// Без него POST-запрос отклоняется с кодом 400 — его надо достать из HTML.
function antiforgeryToken(res) {
  return res.html().find('input[name="__RequestVerificationToken"]').first().attr('value');
}

// Сценарий 1: покупатель просто смотрит товары.
export function browse() {
  group('01 Каталог', () => {
    const res = http.get(`${BASE_URL}/`);
    check(res, { 'каталог открылся': (r) => r.status === 200 && r.body.includes('Все игрушки') });
  });
  sleep(1);

  group('02 Категория «Самолёты»', () => {
    const res = http.get(`${BASE_URL}/?category=${encodeURIComponent('Самолёты')}`);
    check(res, { 'категория открылась': (r) => r.status === 200 && r.body.includes('Альбатрос') });
  });
  sleep(1);

  group('03 Товар «Планер»', () => {
    const res = http.get(`${BASE_URL}/Product/2`);
    check(res, { 'карточка товара': (r) => r.status === 200 && r.body.includes('Вам также может понравиться') });
  });
  sleep(1);

  group('04 Похожий товар', () => {
    const res = http.get(`${BASE_URL}/Product/3`);
    check(res, { 'похожий товар открылся': (r) => r.status === 200 && r.body.includes('Сокол') });
  });
  sleep(1);
}

// Сценарий 2: покупатель кладёт товар в корзину и оформляет заказ.
export function buy() {
  let res = http.get(`${BASE_URL}/Product/5`);
  check(res, { 'товар открылся': (r) => r.status === 200 });
  sleep(1);

  group('05 Добавление в корзину', () => {
    res = http.post(`${BASE_URL}/Product/5?handler=Add`, {
      quantity: '1',
      __RequestVerificationToken: antiforgeryToken(res),
    });
    check(res, { 'товар в корзине': (r) => r.status === 200 && r.url.endsWith('/Cart') });
  });
  sleep(1);

  group('06 Оформление заказа', () => {
    res = http.get(`${BASE_URL}/Checkout`);
    res = http.post(`${BASE_URL}/Checkout`, {
      'Customer.Name': 'Нагрузочный тест',
      'Customer.Email': 'k6@example.com',
      'Customer.Phone': '89123456789',
      'Customer.Address': 'г. Москва, ул. Тестовая, 1',
      __RequestVerificationToken: antiforgeryToken(res),
    });
    check(res, { 'заказ оформлен': (r) => r.status === 200 && r.body.includes('оформлен') });
  });
  sleep(1);
}
