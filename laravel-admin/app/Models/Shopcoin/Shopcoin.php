<?php

namespace App\Models\Shopcoin;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Shopcoin extends Eloquent
{
    use HasFactory;
    protected $fillable = [
        "shop_coin",
        ];
    
}
