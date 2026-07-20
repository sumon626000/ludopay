<?php

namespace App\Models\Player;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Userdata extends Eloquent
{
    use HasFactory;

    protected $fillable = [
        'username',
        'userid',
        'userphone',
        'useremail',
        'photo',
        'points',
        'level',
        'online_multiplayer',
        'friend_multiplayer',
        'tokens_captured',
        'won_streaks',
        'referral_count',
        'referral_users',
        'created_date',
        'spin_date',
        'dailyReward_date',
        'referral_code',
        'connect',
        'winning_amount',
        'status',
        'banned',
        'kyc_status',
        'device_token',
    ];
}
