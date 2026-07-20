<?php

namespace App\Models\Deposit;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Deposit extends Eloquent
{
    use HasFactory;
    protected $collection = 'deposit_manual';
    protected $fillable = [
        "id_dp",
        "username",
        "amount",
        "trxid",
        "method",
        "number",
        "status_dp",
        "date"
        ];
}
