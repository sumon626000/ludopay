<?php

namespace App\Models\Transaction;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Transaction extends Eloquent
{
    use HasFactory;

    protected $fillable = [
        "userid",
        "order_id",
        "txn_id",
        "amount",
        "status",
        "trans_date",
        "created_at",
        ];
}
