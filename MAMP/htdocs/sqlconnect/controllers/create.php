<?php
//
//include "UsersController.php";
//use Controllers\users as C;
//
//$usersObject = new C;
//$usersObject->create($_POST);

include "UsersController.php";
use UsersController\users as T;
$table = new T();
$table->create($_POST);

?>