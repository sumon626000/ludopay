var roomlist = [];
var database = null;
var io;
var roommanager = require('../room_manager/roommanager');
var dateFormat = require("dateformat");
const { emit } = require('nodemon');
var socketlist = [];
var disconnectTimers = {};
const DISCONNECT_GRACE_MS = 60000;

function getDisconnectTimerKey(roomid, userphone) {
    return roomid + '_' + userphone;
}

function clearDisconnectTimer(roomid, userphone) {
    let timerKey = getDisconnectTimerKey(roomid, userphone);
    if (disconnectTimers[timerKey]) {
        clearTimeout(disconnectTimers[timerKey]);
        delete disconnectTimers[timerKey];
    }
}

function removePlayerAfterGrace(roomid, userphone) {
    if (roomlist.length === 0) {
        return;
    }

    let roomIndex = -1;
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == roomid) {
            roomIndex = index;
            break;
        }
    }

    if (roomIndex === -1) {
        return;
    }

    let num = roomlist[roomIndex].playerlist.indexOf(userphone);
    if (num === -1) {
        return;
    }

    roomlist[roomIndex].playerlist.splice(num, 1);
    roomlist[roomIndex].playerphotos.splice(num, 1);
    roomlist[roomIndex].earnScores.splice(num, 1);

    if (roomlist[roomIndex].playerlist.length == 0) {
        roomlist.splice(roomIndex, 1);
        let query = {
            roomID: parseInt(roomid)
        };
        let collection = database.collection('roomdatas');
        collection.deleteOne(query, function (err, removed) {
            if (err) {
                console.log(err);
            } else {
                console.log(roomid, 'room has removed successfully!');
            }
        });
    } else if (roomlist[roomIndex].playerlist.length == 1) {
        console.log("STOP", roomlist[roomIndex].roomid);
        io.sockets.in('r' + roomlist[roomIndex].roomid).emit('GAME_END', {outerphone: userphone});
    }
}

exports.initdatabase = function (db) {
    database = db;
};
exports.addsocket = function (id)
{
    socketlist.push(id);
}
exports.setsocketio = function (socketio) {
    io = socketio;
};

exports.getroomlist = function () {
    return roomlist;
}

exports.addroom = function (r_roomID, r_title, r_creator, r_username, r_seatlimit, r_status, r_game_mode, r_wifi_mode, r_stake_money, r_win_money, socket) {
    let inputplayerlist = [];
    let inputnamelist = [];
    let playerphotos = [];
    let earnScore = [];
    let diceHistory = [];
    let gameobject = {
        roomid: r_roomID,
        title: r_title,
        creator: r_creator,
        username : r_username,
        seatlimit: parseInt(r_seatlimit),
        status: r_status,
        game_mode: r_game_mode,
        wifi_mode: r_wifi_mode,
        stake_money: r_stake_money,
        win_money: r_win_money,
        playerlist: inputplayerlist,
        namelist : inputnamelist,
        playerphotos: playerphotos,
        earnScores: earnScore,
        dice: 1,
        turnuser: '',
        diceHistory: diceHistory,
        turncount: [],
        move_history: {
            status : '',
            mover: '',
            path : ''
        },
    }
    roomlist.push(gameobject);
}

exports.GetRoomPassedTime = function (socket, data) {
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == data.roomid) {
            roomlist[index].passedtime = parseFloat(data.passedtime);
        }
    }
}
exports.playerenterroom = function (roomid, userphone, username, photo, socket) {
    socket.room = 'r' + roomid;
    socket.userphone = userphone;
    //socket.nickname = username;
    console.log("----- player joined in room No: " + roomid + " ------");
    socket.join('r' + roomid);
    
    if (roomlist.length > 0) {
        for (let index = 0; index < roomlist.length; index++) {
            if (roomlist[index].roomid == roomid) {                             
                for (let i = 0; i < roomlist[index].playerlist.length; i++) {
                    let phone = roomlist[index].playerlist[i];                             
                    if (phone == userphone) {                        
                        let mydata = {
                            result: "failed"
                        }
                        console.log('--- userphone ' + userphone + ' joined already in room ---');
                        socket.emit('REQ_ENTER_ROOM_RESULT', mydata);
                        return;
                    }
                }

                roomlist[index].playerlist.push(userphone);
                roomlist[index].namelist.push(username);
                roomlist[index].playerphotos.push(photo);
                roomlist[index].earnScores.push(0);

                exports.GetUserListInRoom(roomid);

                if (roomlist[index].playerlist.length == roomlist[index].seatlimit) {
                    // start game
                    roomlist[index].turnuser = userphone;
                    console.log('----- GameRoom is full players, so GAME START -----');
                    let mydata = {
                        result: "success"
                    }
                    io.sockets.in('r' + roomid).emit('REQ_ENTER_ROOM_RESULT', mydata);
                    roomlist[index].status = "full";
                    UpdateRoomStatus(roomid);
                }
            }
        }
    }

    // roommanager.GetRoomList();
}
exports.reconnectRoom = function (roomid, username, userphone, old_socketID, socket)
{
    clearDisconnectTimer(roomid, userphone);

    let roomindex = 0;
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == roomid) {
            roomindex = index;
        }
    }
    
    let ischeck = roomlist[roomindex].playerlist.filter(function (object) {
        return (object == userphone)
    });

    if(ischeck.length == 0)
    {
        let emitdata = {
            message: "exitUser"
        }
        socket.emit('EXIT_GAME', emitdata);
        console.log("You already got disconnection");
    }
    else
    {
        let oldIndex = socketlist.indexOf(old_socketID);
        if (oldIndex >= 0) {
            socketlist.splice(oldIndex, 1);
        }
        //console.log("reconn", roomid, username);
        socket.room = 'r' + roomid;
        socket.userphone = userphone;
        socket.username = username;
        socket.join('r' + roomid);
        let emit_data = {
            roomid: roomid,
            reconnecter : userphone,
            status: roomlist[roomindex].move_history.status,
            mover: roomlist[roomindex].move_history.mover,
            path: roomlist[roomindex].move_history.path
        }
        io.sockets.in('r' + roomid).emit('RECONNECT_RESULT', emit_data);
    }
}
exports.GetUserListInRoom = function (roomid) {
    let roomindex = 0;
    let mydata = '';
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == roomid) {
            roomindex = index;
        }
    }
    for (let i = 0; i < roomlist[roomindex].namelist.length; i++) {
        mydata = mydata + '{' +
            '"userphone":"' + roomlist[roomindex].playerlist[i] + '",' +
            '"username":"' + roomlist[roomindex].namelist[i] + '",' +
            '"photo":"' + roomlist[roomindex].playerphotos[i] + '",' +
            '"points":"' + 0 + '",' +
            '"level":"' + 0 + '"},';
    }
    mydata = mydata.substring(0, mydata.length - 1);
    mydata = '{' +
        '"result":"success",' +
        '"roomid":"' + roomid + '",' +
        '"userlist": [' + mydata;
    mydata = mydata + ']}';
    //console.log('---REQ_USERLIST_ROOM_RESULT---  ', JSON.parse(mydata));
    io.sockets.in('r' + roomid).emit('REQ_USERLIST_ROOM_RESULT', JSON.parse(mydata));
}
exports.AddHistory = function (data) {
    let collection = database.collection('gamehistorys');
    let currentDate = new Date();
    let currentTime =  dateFormat(currentDate, "dddd mmmm dS yyyy h:MM:ss TT");
    let query = {
        userid: data.userid,
        username : data.username,
        creater: data.creater,
        seat_limit : data.seat_limit,
        game_mode : data.gamemode,
        stake_money : parseInt(data.stake_money),
        game_status : data.game_status,
        win_money : parseInt(data.win_money),        
        playing_time : currentTime,
    };
    collection.insertOne(query, function (err) {
        if (!err) {
            console.log("history info added");
        }
    });
}



function GetThisWeek() {
    let curr = new Date
    let week = []

    for (let i = 1; i <= 7; i++) {
        let first = curr.getDate() - curr.getDay() + i
        let day = new Date(curr.setDate(first)).toISOString().slice(0, 10)
        week.push(day)
        //console.log('*** ', day);
    }
    return week;
}


function msToTime(duration) {
    let milliseconds = parseInt((duration % 1000) / 100),
        seconds = Math.floor((duration / 1000) % 60),
        minutes = Math.floor((duration / (1000 * 60)) % 60),
        hours = Math.floor((duration / (1000 * 60 * 60)) % 24);

    _hours = (hours < 10) ? "0" + hours : hours;
    _minutes = (minutes < 10) ? "0" + minutes : minutes;
    _seconds = (seconds < 10) ? "0" + seconds : seconds;
    console.log("Spin Remaining: ", _hours + ":" + _minutes + ":" + _seconds + "." + milliseconds);
    let datajson = {
        result: "remaining",
        hours: hours,
        minutes: minutes,
        seconds: seconds
    }
    return datajson;
}


exports.GetTurnUser = function (socket, data) {
    console.log("ASK TURN USER");
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == data.roomid) {
            let username = data.username;
            let userphone = data.userphone;
            //console.log(username);
            let ischeck = roomlist[index].turncount.filter(function (object) {
                return (object == userphone)
            });
            //console.log("ischeck: ", ischeck);
            if(ischeck.length == 0)
                roomlist[index].turncount.push(userphone);
            if(roomlist[index].turncount.length == roomlist[index].seatlimit){
                roomlist[index].dice = parseInt(data.dice);
                SetTurn(index, data.roomid);
                //console.log("Decide Turn");
            }
            break;
        }
    }
}

function SetTurn(index, roomid) 
{
    if (roomlist[index].dice < 6){
        let turnuser = roomlist[index].turnuser;
        for (let i = 0; i < roomlist[index].playerlist.length; i++) {
            const element = roomlist[index].playerlist[i];
            if(element == turnuser)
            {
                if(i == roomlist[index].playerlist.length - 1)
                {
                    i = 0;
                }
                else
                {
                    i++;
                }
                turnuser = roomlist[index].playerlist[i];
                roomlist[index].turnuser = turnuser;
            }
        }
    }
    setTimeout(() => {
        if (roomlist[index].playerlist.length > 0) {
            let value = randomNum(1, 6);
            // let value2 = randomNum(1, 3);
            // if(value == 6)
            // {
            //     if(value2 == 1)
            //     {
            //         value = randomNum(1, 5);
            //     }
            // }
            roomlist[index].dice = value;
            let turndata = {
                turnuser: roomlist[index].turnuser,
                dice: roomlist[index].dice
            }
            roomlist[index].turncount = [];
            //io.sockets.in('r' + roomid).emit('REQ_TURNUSER_RESULT', turndata);
            setTimeout(() => {
                io.sockets.in('r' + roomid).emit('REQ_TURNUSER_RESULT', turndata);
            }, 400);
        }
    }, 100);
}

function UpdateRoomStatus(roomid) {
    var collection = database.collection('roomdatas');
    var query = {
        roomID: roomid
    };

    collection.findOne(query, function (err, result) {
        if (err) {
            console.log(err);
        } else {
            collection.updateOne(query, {
                $set: {
                    status: "full"
                }
            }, function (err) {
                if (err) throw err;
            });
        }
    });
}

function randomNum(min, max) {
    var random = Math.floor((Math.random() * (max - min + 1)) + min);
    return random;
}

exports.ChatMessage = function (socket, data) {
    var mydata = {
        result: "success",
        username: data.username,
        message: data.message
    };
    //socket.in('r' + data.roomid).emit('REQ_CHAT_RESULT', mydata); 
    io.sockets.in('r' + data.roomid).emit('REQ_CHAT_RESULT', mydata);
};

exports.Roll_Dice = function (socket, data) {
    var roomid = data.roomid;
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == roomid) {
            if (roomlist[index].dice == data.dice) {
                var mydata = {
                    roller: data.roller,
                    dice: data.dice
                };
                //console.log("REQ_ROLL_DICE_RESULT", roomid, data.roller, data.dice);
                socket.in('r' + roomid).emit('REQ_ROLL_DICE_RESULT', mydata);
                break;
            } else {
                console.log(data.roller, 'is Hacker');
            }
        }
    }
};
exports.Move_Token = function (socket, data) {
    var roomid = data.roomid;
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == roomid) {
            var mydata = {
                status: data.status,
                mover: data.mover,
                path: data.path
            };
            roomlist[index].move_history.status = data.status;
            roomlist[index].move_history.mover = data.mover;
            roomlist[index].move_history.path = data.path;
            socket.in('r' + roomid).emit('REQ_MOVE_TOKEN_RESULT', mydata);
            console.log(roomlist[index].move_history);
            break;
        }
    }
};

exports.Set_Auto = function (socket, data) {
    let roomid = data.roomid;
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == roomid) {
            var mydata = {
                user: data.user,
                auto: data.auto
            };
            socket.in('r' + roomid).emit('REQ_AUTO_RESULT', mydata);
            break;
        }
    }
};
exports.LeaveRoom = function (socket, data) {
    let mydata = {
        result: "success",
        username: data.username,
        userphone:data.userphone,
        message: "user has left the room"
    };

    io.sockets.in('r' + data.roomid).emit('REQ_LEAVE_ROOM_RESULT', mydata);
    // socket.in('r' + data.roomid).emit('REQ_LEAVE_ROOM_RESULT', mydata);
    //socket.leave('r' + data.roomid);
    console.log(data.userphone, "has ", data.roomid, "room exit");

    if (roomlist.length > 0) {
        let removeindex = null;
        for (let index = 0; index < roomlist.length; index++) {
            if (roomlist[index].roomid == data.roomid) {
                let num;
                let isExist = false;
                for (let i = 0; i < roomlist[index].playerlist.length; i++) {
                    if (roomlist[index].playerlist[i] == data.userphone) {
                        isExist = true;
                        num = i
                        break;
                    }
                }
                if (isExist == true) {
                    clearDisconnectTimer(data.roomid, data.userphone);
                    if (roomlist[index].turnuser == data.userphone) {
                        console.log('is changing turn');
                        SetTurn(index, data.roomid);
                    }
                    setTimeout(() => {
                        if(roomlist[index] != undefined){
                            roomlist[index].playerlist.splice(num, 1);
                            roomlist[index].playerphotos.splice(num, 1);
                            roomlist[index].namelist.splice(num, 1);
                            roomlist[index].earnScores.splice(num, 1);
                            //exports.GetUserListInRoom(data.roomid);
                            if (roomlist[index].playerlist.length == 0) {
                                removeindex = index;
                                if (removeindex != null) {
                                    roomlist.splice(removeindex, 1);
                                    let query = {
                                        roomID: parseInt(data.roomid)
                                    }
                                    let collection = database.collection('roomdatas');
                                    collection.deleteOne(query, function (err, removed) {
                                        if (err) {
                                            console.log(err);
                                        } else {
                                            console.log('roomID:' + data.roomid + ' has removed successfully!');
                                        }
                                    });
                                    //roommanager.GetRoomList();
                                }
                            } else if (roomlist[index].playerlist.length == 1) {
                                console.log("STOP! Everyone not me outsided~");
                                io.sockets.in('r' + data.roomid).emit('GAME_END', {outerphone : data.userphone});
                            }
                        }
                    }, 200);
                }
            }
        }

    }
}
exports.RemoveRoom = function(socket, data)
{
    console.log("Remove Force Room", data.roomid);
    let removeindex;
    for (let index = 0; index < roomlist.length; index++) {
        if (roomlist[index].roomid == data.roomid) {
            removeindex = index;
            roomlist.splice(removeindex, 1);
            let query = {
                roomID: parseInt(data.roomid)
            };
            let collection = database.collection('roomdatas');
            collection.deleteOne(query, function (err, removed) {
                if (err) {
                    console.log(err);
                } else {
                    console.log(data.roomid, 'room has removed successfully!');
                }
            });
        }
    }
}
exports.OnDisconnect = function (socket) {
    console.log("---- Disconnect -----", socket.room, socket.userphone, socket.id);
    //let collection = database.collection('userdatas');
    let userdatas = database.collection('userdatas');
    userdatas.updateOne({connect:socket.id}, {
        $set: {
            status: 0,
            login_status:'0'
        }
    }, function (err) {
        if (err) throw err;
    });
    let websettings = database.collection('websettings');
    websettings.findOne({}, function(err, result)
    {
        let webdata ;
        if(err)
            console.log(err);
        if(result != null){
            if(parseInt(result.activeplayer) > 0){
                websettings.updateOne({},{$set:{activeplayer:parseInt(result.activeplayer) - 1}},function(err) {
                    if(err) throw err;                                                
                });
            }            
        }
    });

    let ischeck = socketlist.filter(function (object) {
        return (object == socket.id)
    });
    
    if (ischeck.length == 0) { 
        console.log("re-connected user");
    }
    else{
        socketlist.splice(socketlist.indexOf(socket.id),1);
        let userphone = socket.userphone;
        console.log("  leaving user's phone : ", userphone)
    
        if (socket.room == undefined || userphone == undefined)
            return;
    
        let roomid_arr = socket.room.split("");
        roomid_arr.splice(0, 1);
        let roomid = '';
        for (let i = 0; i < roomid_arr.length; i++) {
            roomid += roomid_arr[i];
        }
        console.log("roomid : ", roomid);
    
        if (roomlist.length > 0) {
            let removeindex = null;
            for (let index = 0; index < roomlist.length; index++) {
                if (roomlist[index].roomid == roomid) {
                    //console.log("yes");
                    let num;
                    let isExist = false;
                    for (let i = 0; i < roomlist[index].playerlist.length; i++) {
                        if (roomlist[index].playerlist[i] == userphone) {
                            isExist = true;
                            //console.log("yes");
                            num = i
                            break;
                        }
                    }
                    if (isExist == true) {
                        clearDisconnectTimer(roomid, userphone);
                        console.log("Grace period started for", userphone, "in room", roomid, "-", DISCONNECT_GRACE_MS / 1000, "seconds");
                        let timerKey = getDisconnectTimerKey(roomid, userphone);
                        disconnectTimers[timerKey] = setTimeout(() => {
                            delete disconnectTimers[timerKey];
                            removePlayerAfterGrace(roomid, userphone);
                        }, DISCONNECT_GRACE_MS);
                    }
    
                }
            }
    
        }
    }
}
function getConnectedList ()
{
    let list = []
    
    for ( let client in io.sockets.connected )
    {
        list.push(client)
    }
    
    return list
}
exports.Pause_Game = function (socket, data)
{
    let roomid = data.roomid;
    let outerName = data.outerName;
    let outerPhone = data.outerPhone;
    let emitdata = {
        roomid : roomid,
        outerName : outerName,
        outerPhone : outerPhone
    }
    socket.in('r' + roomid).emit('REQ_PAUSE_RESULT', emitdata);
}
exports.Resume_Game = function (socket, data)
{
    let roomid = data.roomid;
    let emitdata = {
        roomid : roomid
    }
    socket.in('r' + roomid).emit('REQ_RESUME_RESULT', emitdata);
}