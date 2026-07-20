<?php

namespace App\Models\Admin;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;
use Jenssegers\Mongodb\Eloquent\Model as Eloquent;

class Admin extends Eloquent
{
    use HasFactory;

    public $timestamps = false;

    protected $fillable = [
            "name",
            "username",
            "email",
            "role",
            "password",
            "bio",
            "birthdate",
            "website",
            "phone",
            "country",
            "company",
            "profile_img",
            "work",
            "publish_year",
            "facebook",
            "instagram",
            "twitter",
            "linkedin",
            "youtube",
            "whatsapp",
    ];
}
