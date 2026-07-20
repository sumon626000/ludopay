
var gamemanager = require('../game_manager/gamemanager');
var dateFormat = require("dateformat");
var database = null;
var io;

exports.initdatabase = function(db) {
    database = db;
};

exports.setsocketio=function (socketio) {
    io = socketio;
};

exports.Check_Rooms = function(socket, data)
{
    console.log('----Check Rooms----');
    console.log(data.seat_limit, data.game_mode, data.wifi_mode, data.stake_money, data.win_money);
    //data.seat_limit, data.game_mode, data.wifi_mode, data.stake_money, data.win_money
    
    let collection = database.collection('roomdatas');
    collection.find().toArray(function(err, docs){
        if(err){
            console.log(err);
            let mydata = {
                result : 'failed',
            }
            console.log(1);
            socket.emit('REQ_CHECK_ROOMS_RESULT', mydata);
        }
        else{
            if(docs.length > 0){
                // Let's check that wifi_mode is same as game_mode or not.
                let rooms_wifi = docs.filter(function (object) {
                    return ((object.wifi_mode == data.wifi_mode) && (object.game_mode == data.game_mode)
                    && (object.seat_limit == data.seat_limit) && (object.stake_money == data.stake_money)
                    && (object.win_money == data.win_money))
                });
                // console.log(rooms_wifi);
                if(rooms_wifi.length > 0)
                {
                    let exitRoomId = -1;
                    for (let index = 0; index < rooms_wifi.length; index++) {
                        if(rooms_wifi[index].status != "full")
                        {
                            exitRoomId = index;
                            break;
                        }
                    }
                    if(exitRoomId != -1)
                    {
                        console.log(2);
                        let mydata = {
                            result : 'success',
                            roomID : rooms_wifi[exitRoomId].roomID
                        };
                        socket.emit('REQ_CHECK_ROOMS_RESULT', mydata);
                    }
                    else
                    {
                        console.log(3);
                        let mydata = {
                            result : 'failed',
                        }
                        socket.emit('REQ_CHECK_ROOMS_RESULT', mydata);     
                    }
                }
                else
                {
                    console.log(4);
                    let mydata = {
                        result : 'failed',
                    }
                    socket.emit('REQ_CHECK_ROOMS_RESULT', mydata);                     
                }
            }
            else
            {
                console.log(5);
                let mydata = {
                    result : 'failed',
                }
                socket.emit('REQ_CHECK_ROOMS_RESULT', mydata);        
            }
        }
    });
};

exports.CreateRoom = function(socket, userInfo)
{
    let collection = database.collection('roomdatas');
    //(data.seat_limit, data.game_mode, data.wifi_mode, data.stake_money, data.win_money);
    collection.find().sort({roomID:-1}).limit(1).toArray(function(err, docs) {
        if(err)
            throw err;
        else
        {
            let userdatas = database.collection('userdatas');
            let filter = {userphone : userInfo.userphone};
            
            userdatas.findOne(filter, function(err, result)
            {
                if(err)
                    console.log(err);
                else if (!result)
                {
                    console.log("---- Failed Create Room, user not found ---- ");
                    socket.emit('REQ_CREATE_ROOM_RESULT', { result : 'failed' });
                }
                else
                {
                    if(parseInt(result.points) < parseInt(userInfo.stake_money)){    
                        console.log("---- Failed Create Room, Because not enough money ---- ");                    
                        var mydata = {
                            result : 'failed',                            
                        };
                        socket.emit('REQ_CREATE_ROOM_RESULT', mydata);                        
                    }
                    else
                    {
                        let id = 1;
                        if (docs[0])
                            id = docs[0].roomID + 1;
                        let currentTime = new Date();
                        let timel =  dateFormat(currentTime, "dddd mmmm dS yyyy h:MM:ss TT");  
                        let query = {
                            roomID : id,
                            title : userInfo.room_title,
                            creator: userInfo.userphone,
                            username : userInfo.username,
                            seat_limit : parseInt(userInfo.seat_limit),
                            status : userInfo.status,
                            game_mode : userInfo.game_mode,
                            wifi_mode : userInfo.wifi_mode,
                            stake_money : parseInt(userInfo.stake_money),
                            win_money : parseInt(userInfo.win_money),
                            create_time : timel,
                        };
                        
                        collection.insertOne(query,function(err) {
                            if (err) {
                                console.log(err);
                                throw err;
                            }
                            else
                            {
                                console.log("---- Success Create Room ---- ");                    
                                var mydata = {
                                    result : 'success',
                                    roomID : id
                                };
                                socket.emit('REQ_CREATE_ROOM_RESULT', mydata);
                                gamemanager.addroom(id, userInfo.room_title, userInfo.userphone, userInfo.username, parseInt(userInfo.seat_limit), userInfo.status, 
                                    userInfo.game_mode, userInfo.wifi_mode,userInfo.stake_money, userInfo.win_money, socket);
                            }
                        });
                    }
                }
            });
        }
    });
}

exports.JoinRoom = function(socket, data)
{
    let userdatas = database.collection('userdatas');
    let filter = {userphone : data.userphone};
    userdatas.findOne(filter, function(err, result)
    {
        if(err)
            console.log(err);
        else if (!result)
        {
            console.log("---- Failed Join Room, user not found ---- ");
            socket.emit('REQ_JOIN_ROOM_RESULT', { result : 'failed' });
        }
        else
        {
            if(parseInt(result.points) < parseInt(data.stake_money)){                        
                var mydata = {
                    result : 'failed',
                };
                socket.emit('REQ_JOIN_ROOM_RESULT', mydata);
            }
            else
            {
                gamemanager.playerenterroom(parseInt(data.roomID),data.userphone, data.username, data.photo, socket);
            }
        }
    });
}
exports.ReJoinRoom = function(socket, data)
{
    gamemanager.reconnectRoom(parseInt(data.roomid),data.username, data.userphone, data.old_socketID, socket);
}
exports.GetRoomInfo = function(socket, data)
{
    let roomid = data.roomID;    
    let roomlist = gamemanager.getroomlist();
    let isThere = false;
    for (let index = 0; index < roomlist.length; index++) {
        if(roomlist[index].roomid == roomid)
        {
            let mydata = {
                seatlimit: roomlist[index].seatlimit,
                gamemode: roomlist[index].game_mode,
                stakemoney: roomlist[index].stake_money,
                winmoney : roomlist[index].win_money
            };
            socket.emit('REQ_ROOM_INFO_RESULT', mydata);
            isThere = true;
            break;
        }
    }
    if(!isThere)
    {
        let mydata = {
            seatlimit: 0,
            stakemoney: "0",
            winmoney : "0"
        };
        socket.emit('REQ_ROOM_INFO_RESULT', mydata);
    }
}
exports.GetRoomList = function()
{
    let roomlist = gamemanager.getroomlist();
    let mydata = '';
    // var collection = database.collection('roomdatas');
    // collection.find().toArray(function(err, docs){
    //     if(err) throw err;
    //     else{
    //         if(docs.length > 0)
    //         {
    //             docs.forEach(element => {
                    
    //             });
    //         }
    //     }
    // });
    for (let i = 0; i < roomlist.length; i++) {
        let currentPlayers = roomlist[i].playerlist.length;
        mydata = mydata + '{' +
            '"roomid":"' + roomlist[i].roomid + '",' +
            '"title":"' + roomlist[i].title + '",' +
            '"seatlimit":"' + roomlist[i].seatlimit + '",' +
            '"type":"' + roomlist[i].type + '",' +
            '"difficulty":"' + roomlist[i].difficulty + '",' +
            '"currentplayers":"' + currentPlayers + '"},';
    }
    mydata=mydata.substring(0,mydata.length-1);
    mydata = '{'
        +'"result" : "success",'
        +'"rooms"  : ['+mydata;
    mydata=mydata+']}';
    //console.log("REQ_ROOM_LIST_RESULT");
    io.sockets.emit('REQ_ROOM_LIST_RESULT', JSON.parse(mydata));
}

exports.CheckRefferal = function(socket, data)
{
    let refer_amount = 0;
    let requester = data.userphone;
    let code = data.referral;
    let websettings = database.collection('websettings');
    websettings.findOne({}, function(err, result)
    {
        if(err)
        {
            console.log(err);            
        }
        else
        {
            if(result != null)
            {
                refer_amount = parseInt(result.refer_bonus) ;
                let collection = database.collection('userdatas');
                collection.find().toArray(function(err, docs){
                    if(!err){
                        if(docs.length > 0){
                            let users = docs.filter(function (object) {
                                return (object.referral_code == code)
                            });
                            
                            if(users.length > 0)
                            {
                                let emitdata = {result : "success", refer_amount : refer_amount};
                                socket.emit('REQ_CHECK_REFFERAL_RESULT', emitdata);
                                let referral_users = [];
                                referral_users = users[0].referral_users;
                                referral_users.push(requester);
                                let query2 = {userphone : users[0].userphone};
                                collection.updateOne(query2, {
                                    $set: {
                                        points: parseInt(users[0].points) + refer_amount,
                                        refer_earning:parseInt(users[0].refer_earning) + refer_amount,
                                        referral_count : users[0].referral_count + 1,
                                        referral_users : referral_users
                                    }
                                }, function (err) {
                                    if (err) throw err;
                                });
                            }
                            else
                            {
                                let emitdata = {result : "failed"};
                                socket.emit('REQ_CHECK_REFFERAL_RESULT', emitdata);
                            }
                        }
                    }
                });


            }
        }
    });
}
