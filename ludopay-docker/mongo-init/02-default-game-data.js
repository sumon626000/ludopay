db = db.getSiblingDB('webplustechludo');

if (db.shopcoins.countDocuments() === 0) {
  db.shopcoins.insertMany([
    { shop_coin: 100 },
    { shop_coin: 500 },
    { shop_coin: 1000 },
    { shop_coin: 5000 },
    { shop_coin: 10000 }
  ]);
  print('shopcoins seeded');
}

if (db.bids.countDocuments() === 0) {
  db.bids.insertMany([
    { bid_value: 10 },
    { bid_value: 50 },
    { bid_value: 100 },
    { bid_value: 500 },
    { bid_value: 1000 }
  ]);
  print('bids seeded');
}

if (db.websettings.countDocuments() === 0) {
  db.websettings.insertOne({
    _id: ObjectId('60bed6aef3c80e44a06e01f0'),
    website_name: 'Monster Game',
    website_url: 'http://127.0.0.1:8000',
    website_tagline: 'Monster Game',
    skin_mode: '',
    sideskin_mode: 'menu-light',
    copyright: 'Monster Game',
    activeplayer: 0,
    signup_bonus: 100,
    refer_bonus: 50,
    min_withdraw: 100,
    commission: 0,
    bot_status: 0,
    support_mail: 'admin@gmail.com'
  });
  print('websettings seeded');
}

print('Default game data OK');
