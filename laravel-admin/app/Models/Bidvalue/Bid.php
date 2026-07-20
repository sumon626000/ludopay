<?php

namespace App\Models\Bidvalue;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Bid extends Eloquent
{
    use HasFactory;

    protected $fillable = [
        "bid_value",
        ];
}
