db = db.getSiblingDB('webplustechludo');

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
    bot_status: 0
  });
  print('websettings seeded');
}
