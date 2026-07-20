<?php

namespace App\Models\Withdraw;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Withdraw extends Eloquent
{
    use HasFactory;

    protected $fillable = [
        "userid",
        "amount",
        "payment_method",
        "wallet_number",
        "bank_name",
        "account_number",
        "ifsc_code",
        "status",
        "transaction_id",
        "created_at",
        ];
}
