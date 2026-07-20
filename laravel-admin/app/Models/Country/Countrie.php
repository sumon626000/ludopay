<?php

namespace App\Models\Country;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Countrie extends Eloquent
{
    use HasFactory;
    protected $fillable = [
      "sortname",
      "name",
      "phonecode",
    ];
}
