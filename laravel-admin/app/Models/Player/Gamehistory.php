<?php

namespace App\Models\Player;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Gamehistory extends Eloquent
{
    use HasFactory;
    protected $table = "gamehistorys";

    protected $fillable = [
          "userid",
          "username",
          "creater",
          "seat_limit",
          "game_mode",
          "stake_money",
          "game_status",
          "win_money",
          "playing_time",
    ];
}
