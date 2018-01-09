<?php

$db = new SQLite3("migration.db");

$cities   = json_decode(file_get_contents("cities.json"));
$defs     = json_decode(file_get_contents("defs.json"));
$emails   = json_decode(file_get_contents("emails.json"));
$img_urls = json_decode(file_get_contents("img_urls.json"));


foreach ($emails as $key => $value) {
    $query = "insert into Emails (Id, EmailAddress) values (:id, :email)";
    $stmt = $db->prepare($query);
    $stmt->bindValue(':id', $key);
    $stmt->bindValue(':email', $value);
    $stmt->execute();
}


foreach ($cities as $key => $value) {
    $query = "insert into UserLocations (Id, City, CityId, Lat, Lng, TimeZone) values (:id, :city, :cityid, :lat, :lng, :timezone)";
    $stmt = $db->prepare($query);
    $stmt->bindValue(':id', $key);
    $stmt->bindValue(':city', $value->city);
    $stmt->bindValue(':cityid', $value->id);
    $stmt->bindValue(':lat', $value->lat);
    $stmt->bindValue(':lng', $value->lon);
    $stmt->bindValue(':timezone', $value->timezone);
    $stmt->execute();
}

foreach ($defs as $key => $value) {
    $query = "insert into Defs (Id, Def) values (:id, :def)";
    $stmt = $db->prepare($query);
    $stmt->bindValue(':id', $key);
    $stmt->bindValue(':def', $value);
    $stmt->execute();
}

foreach ($img_urls as $key => $value) {
    $query = "insert into Images (Id, FilePath) values (:id, :filepath)";
    $stmt = $db->prepare($query);
    $stmt->bindValue(':id', $key);
    $stmt->bindValue(':filepath', $value);
    $stmt->execute();
}

