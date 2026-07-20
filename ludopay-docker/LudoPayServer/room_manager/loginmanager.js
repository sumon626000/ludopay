const publicIp = require('public-ip');
var fs = require('fs');
var dateFormat = require("dateformat");
var database = null;
var gamemanager = require('../game_manager/gamemanager');
var serverip = '52.66.210.17';
var port = '16000';

exports.initdatabase = function(db) {
    database = db;
    (async () => {
        // console.log(await publicIp.v4());
        //serverip = await publicIp.v4();
        console.log(serverip);
        //=> '46.5.21.123'
     
        //console.log(await publicIp.v6());
        //=> 'fe80::200:f8ff:fe21:67cf'
    })();
};

exports.LogIn = function (socket,  userInfo) 
{
    var collection = database.collection('userdatas');
    var query = {userphone: userInfo.userphone};
    collection.findOne(query, function(err, result)
    {
        if(err)
            console.log(err);
        else
        {
            var mydata;
            if(result == null){
                mydata = {
                  result:'failed'  
                };
            }
            else
            {
                var inputPass = userInfo.userpassword || '';
                var storedPass = result.password || '';
                if (storedPass != inputPass) {
                    mydata = {
                        result:'failed'  
                    };
                    socket.emit('GET_LOGIN_RESULT', mydata);
                    return;
                }

                collection.updateOne(query,{$set:{connect:socket.id, status:1, login_status:'1'}},function(err) {
                    if(err) throw err;
                    //else
                    //console.log('- User socket_id:', socket.id);
                });             
                
                let websettings = database.collection('websettings');
                websettings.findOne({}, function(err, result)
                {
                    let webdata ;
                    if(err)
                        console.log(err);
                    if(result != null){
                        if(parseInt(result.activeplayer) >= 0){
                            websettings.updateOne({},{$set:{activeplayer:parseInt(result.activeplayer) + 1}},function(err) {
                                if(err) throw err;                            
                                else console.log('one player logined also. :) +1');
                            });
                        }
                    }
                });

                var mydata = {
                    result : 'success',
                    username : result.username, 
                    userid : result.userid,                    
                    pass : result.password,
                    userphone: result.userphone,
                    useremail: result.useremail,
                    photo: result.photo,
                    points : result.points, 
                    level : result.level,
                    online_multiplayer: result.online_multiplayer,
                    friend_multiplayer: result.friend_multiplayer,
                    tokens_captured : result.tokens_captured,
                    won_streaks : result.won_streaks,
                    referral_count : result.referral_count,
                    referral_code: result.referral_code,
                    ant : result.winning_amount,
                    kyc_status : result.kyc_status,
                }
                //console.log('---' + result.username + ' s LOGIN INFO ---' , mydata);
            }
            socket.emit('GET_LOGIN_RESULT', mydata);
        }
    });
}

exports.LogOut = function (socket,  data) 
{
    var collection = database.collection('userdatas');
    var query = {userphone: data.phone};
    collection.findOne(query, function(err, result)
    {
        if(err)
            console.log(err);
        else
        {
            var mydata;
            if(result == null){
                console.log('logout failed ---');
            }
            else
            {
                collection.updateOne(query,{$set:{connect:socket.id, status:1, login_status:'0'}},function(err) {
                    if(err) throw err;
                    //else
                    //console.log('- User socket_id:', socket.id);
                });
            }
        }
    });
}

exports.SignUp = function (socket, data)
{
    let collection = database.collection('websettings');    
    let signup_bonus = 0;
    collection.findOne({}, function(err, result)
    {
        if(err)
            console.log(err);
        else
        {
            if(result != null){
                signup_bonus = parseInt(result.signup_bonus) || 0;
            }
            {
                var collection = database.collection('userdatas');
                var randomnum1 = '' + Math.floor(100000 + Math.random() * 900000);
                var randomnum2 = '' + Math.floor(100000 + Math.random() * 900000);
                var randomnum = randomnum1 + randomnum2;
                var referralCode = ''+ Math.floor(100000 + Math.random() * 900000);
                var name = 'Guest' + randomnum2;
                var password = data.userpassword;
                if(data.signtype == 'google' || data.signtype == 'facebook' || data.signtype == 'phone')
                    name = data.username; 
                if(data.signtype == 'google' || data.signtype == 'facebook' || data.signtype == 'guest')
                    password = "";
                    
                var phone = data.userphone;
                var photo = data.userphoto;                
                var email = data.useremail;
                var online_multiplayer = {played : 0, won : 0};
                var friend_multiplayer = {played : 0, won : 0};
                var tokens_captured = {mine : 0, opponents : 0};
                var won_streaks = {current : 0, best : 0};
                let currentTime = new Date();
                let timel =  dateFormat(currentTime, "dddd mmmm dS yyyy h:MM:ss TT");                
                var user_data = {
                    username : name,
                    userid : randomnum, 
                    userphone: phone,
                    useremail : email,
                    password : password,
                    photo: photo,
                    points : signup_bonus, 
                    level : 0, 
                    online_multiplayer: online_multiplayer,
                    friend_multiplayer: friend_multiplayer,
                    tokens_captured : tokens_captured,
                    won_streaks : won_streaks,
                    referral_count : 0,
                    referral_users : [],
                    created_date : timel,
                    spin_date : new Date(),
                    dailyReward_date : new Date(),
                    referral_code: referralCode,
                    connect : socket.id,
                    winning_amount : 0,
                    refer_earning : 0,
                    status : 1,
                    login_status:"1",
                    banned : 1,
                    kyc_status : 0,
                    device_token:data.device_token,
                    used_refer_code:''                    
                };
                
                collection.insertOne(user_data);

                let websettings = database.collection('websettings');
                websettings.findOne({}, function(err, result)
                {
                    let webdata ;
                    if(err)
                        console.log(err);
                    if(result != null){
                        if(parseInt(result.activeplayer) >= 0)
                        {
                            websettings.updateOne({},{$set:{activeplayer:parseInt(result.activeplayer) + 1}},function(err) {
                                if(err) throw err;                            
                                else console.log('one player logined also. :) +1');
                            });
                        }
                    }
                });

                var mydata = {
                    result : 'success',
                    userphone: phone,
                    username : name, 
                    userid : randomnum, 
                    pass : password,
                    points : signup_bonus, 
                    referral_code: referralCode
                }
                console.log("- New user: " + name + " has Registered.");
                socket.emit('GET_REGISTER_RESULT', mydata);
            }
        }
    }); 
}


exports.ChangePassword = function (socket,  data) 
{
    var collection = database.collection('userdatas');
    var query = {userphone: data.userphone};
    collection.findOne(query, function(err, result)
    {
        if(err)
            console.log(err);
        else
        {
            var mydata;
            if(result == null){
                mydata = {
                  result:'failed'  
                };
            }
            else
            {
                collection.updateOne(query,{$set:{password:data.newpassword}},function(err) {
                    if(err) throw err;
                    //else
                    //console.log('- User socket_id:', socket.id);
                });
                mydata = {
                                result : 'success',
                                newpassword : data.newpassword
                             };                             
            }            
            socket.emit('GET_CHANGEPASS_RESULT', mydata);
        }
    });
}

exports.Insert_KYC = function(data, socket){
    let currentTime = new Date();
    let timel =  dateFormat(currentTime, "dddd mmmm dS yyyy h:MM:ss TT");  
    var kyc_data = {
        document_number : data.document_number,
        first_name : data.first_name,
        last_name : data.last_name,
        dob : data.dob,
        document_image: data.document_image,
        document_type : data.document_type,
        verification_status : '0',
        userid : data.userid,
        created_at : timel
    };
    console.log('kyc----- : ' , kyc_data);
    var collection = database.collection('kycdetails');
    collection.find().toArray(function(err, docs){
        if(err){
            throw err;
        }
        else{
            if(docs.length > 0){
                var kyc_infolist = docs.filter(function (object) {
                    return (object.userid == data.userid)
                });
                // console.log(rooms_wifi);
                if(kyc_infolist.length > 0)
                {
                    console.log('already exists your KYC Document');                    
                    var mydata = {
                        result:'failed'
                    };
                    socket.emit('REQ_KYC_RESULT', mydata);  
                }
                else
                {
                    console.log('KYC inserted');
                    collection.insertOne(kyc_data);
                    var mydata = {
                        result:'success'
                    };
                    socket.emit('REQ_KYC_RESULT', mydata);                     
                }
            }
            else
            {
                console.log('---KYC inserted---');
                collection.insertOne(kyc_data);
                var mydata = {
                    result:'success'
                };
                socket.emit('REQ_KYC_RESULT', mydata);        
            }
        }
    });
}

exports.Check_KYC = function(data, socket){    
    var collection = database.collection('kycdetails');
    collection.find().toArray(function(err, docs){
        if(err){
            throw err;
        }
        else{
            if(docs.length > 0){
                var kyc_infolist = docs.filter(function (object) {
                    return (object.userid == data.userid)
                });
                if(kyc_infolist.length > 0)
                {                    
                    var mydata = {
                        result : 'success',
                        status :kyc_infolist[0].verification_status,
                    };
                    socket.emit('REQ_CHECK_KYC_RESULT', mydata);
                }
                else
                {
                    var mydata = {
                        result:'failed'
                    };
                    socket.emit('REQ_CHECK_KYC_RESULT', mydata);                     
                }
            }
            else
            {
                var mydata = {
                    result:'failed'
                };
                socket.emit('REQ_KYC_RESULT', mydata);        
            }
        }
    });
}

exports.Insert_Withdraw = function(data, socket){    
    var withdraw = database.collection('withdraws');
    var userdata = database.collection('userdatas');    
    var query = {userid : data.userid}
    let currentTime = new Date();
    let timel =  dateFormat(currentTime, "dddd mmmm dS yyyy h:MM:ss TT");  
    var withdrawData = {
        userid : data.userid,
        amount : data.amount,
        payment_method : data.payment_method,
        wallet_number : data.wallet_number,
        bank_name : data.bank_name,
        account_number : data.account_number,
        ifsc_code : data.ifsc_code,
        withdraw_status : '0',
        created_at : timel,        
    }
    userdata.findOne(query, function(err, result){
        if(err)
            console.log(err);
        else
        {
            if(result != null)
            {                
                if(parseInt(data.amount)  <=  parseInt(result.winning_amount))
                {
                    var currentPoints = parseInt(result.points) - parseInt(data.amount);
                    var currentWinAmount = parseInt(result.winning_amount) - parseInt(data.amount);
                    var query_my = {userid : data.userid};
                    userdata.updateOne(query_my,{$set:{points : currentPoints, winning_amount : currentWinAmount }},function(err) {
                        if(err) throw err;
                    });

                    withdraw.insertOne(withdrawData);                    
                    var mydata = {
                        result : 'success'
                    }
                    socket.emit('REQ_WITHDRAW_RESULT', mydata);
                }
                else
                {                    
                    var mydata = {
                        result : 'failed'
                    }
                    socket.emit('REQ_WITHDRAW_RESULT', mydata);
                }
            }
        }
    });
}

exports.Valid_Phone = function(socket, data)
{
    var collection = database.collection('userdatas');
    collection.find().toArray(function(err, docs){
        if(err){
            throw err;
        }
        else{
            if(docs.length > 0){
                var rooms_wifi = docs.filter(function (object) {
                    return (object.userphone == data.phone)
                });
                // console.log(rooms_wifi);
                if(rooms_wifi.length > 0)
                {
                    console.log('already exist user');
                    var mydata = {
                        result:'failed'
                    }
                    socket.emit('REQ_VALID_PHONE_RESULT', mydata);  
                }
                else
                {
                    console.log('success');
                    var mydata = {
                        result:'success'
                    }
                    socket.emit('REQ_VALID_PHONE_RESULT', mydata);                     
                }
            }
            else
            {
                console.log('success');
                var mydata = {
                    result:'success'
                }
                socket.emit('REQ_VALID_PHONE_RESULT', mydata);        
            }
        }
    });
}

exports.Get_Coins = function(data, socket)
{
    var collection = database.collection('userdatas');
    var query = {userphone : data.userphone};
    //console.log('userphone:  ' , data.userphone);
    collection.findOne(query, function(err, result)
    {
        if(err)
        {
            console.log(err);            
        }
        else
        {
            var mydata;
            if(result == null){
                mydata = {
                    result : "failed"
                }
            }
            else
            {
                mydata = {
                    result : 'success',
                    points : result.points,
                    winning_amount : result.winning_amount,
                }
            }
            //console.log('---- REQ_COIN_RESULT ----', mydata);
            socket.emit('REQ_COIN_RESULT', mydata);
        }
    });
}

exports.GetUserInfo = function(socket, userInfo)
{
    //console.log(userInfo.username);
    var collection = database.collection('userdatas');
    var query = {userphone : userInfo.userphone};
    collection.findOne(query, function(err, result)
    {
        if(err)
        {
            console.log(err);
            
        }
        else
        {
            //console.log("- Login userinfo :");
            //console.log(result);
            var mydata;
            if(result == null){
                mydata = {
                    result : "failed"
                }
            }
            else
            {
                mydata = {
                    result : 'success',
                    username : result.username, 
                    userphone : result.userphone,
                    userid : result.userid,
                    photo: result.photo,
                    points : result.points, 
                    level : result.level,
                    online_multiplayer: result.online_multiplayer,
                    friend_multiplayer: result.friend_multiplayer,
                    tokens_captured : result.tokens_captured,
                    won_streaks : result.won_streaks,
                    referral_code : result.referral_code,
                    referral_count : result.referral_count
                }
            }
            socket.emit('GET_USERINFO_RESULT', mydata);
        }
    });
}
exports.UpdateUserInfo = function(socket, userInfo)
{
    //console.log("update user info", userInfo);
    var collection = database.collection('userdatas');
    var query = {userphone: userInfo.userphone};
    var online_multiplayer = {played : parseInt(userInfo.online_played), won : parseInt(userInfo.online_won)};
    var friend_multiplayer = {played : parseInt(userInfo.friend_played), won : parseInt(userInfo.friend_won)};
    var tokens_captured = {mine : parseInt(userInfo.tokenscaptured_mine), opponents : parseInt(userInfo.tokenscaptured_opponents)};
    var won_streaks = {current : parseInt(userInfo.wonstreaks_current), best : parseInt(userInfo.wonstreaks_best)};    
    
   

    collection.findOne(query, function(err, result)
    {
        if(err) console.log(err);
        else
        {                           
            var amount = parseInt(userInfo.winning_amount) +  parseInt(result.winning_amount);
            var data = {
                points:parseInt(userInfo.points),
                level:parseInt(userInfo.level),
                online_multiplayer: online_multiplayer,
                friend_multiplayer: friend_multiplayer,
                tokens_captured : tokens_captured,
                won_streaks : won_streaks,                        
                winning_amount : amount,
            };

            collection.updateOne(query,{$set:data},function(err) {
                if(err) throw err;
                else
                    socket.emit('REQ_UPDATE_USERINFO_RESULT', {result : 'success', amount : amount});
            });
        }
    });
}
exports.Get_User_Photo = function(info, socket)
{
     var buf = Buffer.from(info.photo_data, 'base64');
     fs.writeFile('./delux/userphotos/' + info.userid + '.png', buf, function(err){
        if(err) throw err;
        console.log('Photo Saved!');
        var collection = database.collection('userdatas');
        var url = 'http://'+ serverip + ':' + port + '/userphotos/' + info.userid + '.png';
        collection.updateOne({userid:info.userid},{$set:{photo:url}},function(err) {
            if(err) throw err;
            else
                socket.emit('UPLOAD_USER_PHOTO_RESULT');
        });
     });
}
exports.Get_Lic_Photo = function(info, socket)
{
     var buf = Buffer.from(info.photo_data, 'base64');
     fs.writeFile('./delux/kycPhotos/' + info.userid + '.png', buf, function(err){
        if(err) throw err;
        console.log('kyc Photo Saved!');        
        var url = 'http://'+ serverip + ':' + port + '/kycPhotos/' + info.userid + '.png';
        
        var mydata = {
            photo_url : url,
        }

        socket.emit('UPLOAD_LIC_PHOTO_RESULT', mydata);        
     });
}

exports.GetShopItems = function (socket) {
    let collection = database.collection('shopcoins');
    query = {};
    collection.find().toArray(function(err, docs){
        if(err){
            throw err;
        }
        else{
            if(docs.length > 0){
                //console.log('----- shopitems -----', docs);
                var mydata = {
                    result : 'success',
                    shopitems :  docs,
                    itemlength : docs.length,
                };
                
                socket.emit('REQ_SHOPITEMS_RESULT', mydata);
            }
            else
            {
                var mydata = {
                    result : 'failed',
                };

                socket.emit('REQ_SHOPITEMS_RESULT', mydata);
            }
        }
    });
}

exports.GetSpecialInfo = function (socket) {
    let collection = database.collection('specials');
    query = {};
    collection.find().toArray(function(err, docs){
        if(err){
            throw err;
        }
        else{
            if(docs.length > 0){
                //console.log('----- special items -----', docs);
                var mydata = {
                    result : 'success',
                    offeritems :  docs,
                    itemlength : docs.length,
                };

                socket.emit('REQ_SPECIAL_RESULT', mydata);
            }
            else
            {
                var mydata = {
                    result : 'failed',
                };

                socket.emit('REQ_SPECIAL_RESULT', mydata);
            }
        }
    });
}

exports.GetGameBids = function (socket) {
    let collection = database.collection('bids');    
    collection.find().toArray(function(err, docs){
        if(err){
            throw err;
        }
        else{
            if(docs.length > 0){
                //console.log('----- bid items -----', docs);
                var mydata = {
                    result : 'success',
                    bids :  docs,
                    itemlength : docs.length,
                };

                socket.emit('REQ_BIDS_RESULT', mydata);
            }
            else
            {
                var mydata = {
                    result : 'failed',
                };

                socket.emit('REQ_BIDS_RESULT', mydata);
            }
        }
    });
}

exports.GetGameSettings = function (socket) {
    let collection = database.collection('websettings');
    query = {};
    collection.findOne(query, function(err, result)
    {
        if(err)
            console.log(err);
        else
        {
            if(result == null){
                mydata = {
                  result:'failed'  
                };
                socket.emit('REQ_GAMESETTINGS_RESULT', mydata);
            }
            else
            {
                mydata = {
                    result:'success',
                    commission : result.commission,
                    min_withdraw : result.min_withdraw,
                    refer_bonus : result.refer_bonus,
                    whatsapp_link : result.whatsapp_link,
                    youtube_link : result.youtube_link,
                    bot_mode : result.bot_status,
                    purchase_link : result.purchase_link,
                    privacy_desc : result.privacy_desc,
                    terms_desc : result.terms_desc,
                };
                //console.log('GAMESETTING_____    ' , mydata);
                socket.emit('REQ_GAMESETTINGS_RESULT', mydata);
            }
        }
    });
}


exports.GetWalletHistories = function (socket, data) {
    let transactions = database.collection('transactions');       
    transactions.find().toArray(function(err, docs){
        if(err){
            throw err;
        }
        else
        {
            if(docs.length > 0){
                var deposits = docs.filter(function (object) {
                    return (object.userid == data.userid)
                });
                
                if(deposits.length > 0)
                {
                    let collection = database.collection('withdraws');
                    collection.find().toArray(function(err, docs){
                        if(err){
                            throw err;
                        }
                        else
                        {
                            if(docs.length > 0){
                                var withdraws = docs.filter(function (object) {
                                    return (object.userid == data.userid)
                                });
                                
                                if(withdraws.length > 0)
                                {                                  
                                    //console.log('deposits : ', deposits);   
                                    //console.log('withdraws : ', withdraws); 
                                                       
                                    var mydata = {
                                        result : 'success',
                                        deposits : deposits,
                                        withdraws : withdraws,
                                        deposit_length : deposits.length,
                                        withdraw_length : withdraws.length                                   
                                    };
                                    socket.emit('REQ_WALLET_HIS_RESULT', mydata);  
                                }
                            }
                            else
                            {
                                var mydata = {
                                    result:'failed'
                                };
                                socket.emit('REQ_WALLET_HIS_RESULT', mydata);        
                            }
                        }
                    });
                }
            }
            else
            {
                var mydata = {
                    result:'failed'
                };
                socket.emit('REQ_WALLET_HIS_RESULT', mydata);        
            }
        }
    });
}

