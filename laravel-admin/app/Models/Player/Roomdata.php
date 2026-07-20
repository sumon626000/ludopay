<?php

namespace App\Models\Player;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Roomdata extends Eloquent
{
    use HasFactory;

    protected $fillable = [
          "roomID",
          "title",
          "creator",
          "username",
          "seat_limit",
          "status",
          "game_mode",
          "wifi_mode",
          "stake_money",
          "win_money",
    ];

}
