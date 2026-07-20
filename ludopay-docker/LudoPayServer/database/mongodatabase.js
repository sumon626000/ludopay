var MongoClient = require('mongodb').MongoClient;
var URL = process.env.MONGO_URL || 'mongodb://localhost:27017/webplustechludo';
//var URL = 'mongodb://l6v2Admin:x7ZmTqmgV4tRn327@159.65.150.117:27017/Ludo6';
var state = {
    db: null,
};

exports.connect = function(done) {
    if (state.db)
        return done();

    MongoClient.connect(URL,{ useNewUrlParser: true, useUnifiedTopology: true }, function(err, client) {
        if (err)
            return done(err);
        var db = client.db('webplustechludo');
        state.db = db;
        done();
    });
};

exports.get = function() {
    return state.db;
};

exports.close = function(done) {
    if (state.db) {
        state.db.close(function(err, result) {
            state.db = null;
            state.mode = null;
            done(err);
        });
    }
};