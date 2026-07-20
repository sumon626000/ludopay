<?php

namespace App\Models\Special;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;
class Special extends Eloquent
{
    use HasFactory;

    protected $fillable = [
        "offer_title",
        "add_offer_coin",
        "user_received_coin",
        "offerimage",
        ];
}
