<?php

ini_set( 'error_reporting', E_ALL );
ini_set( 'display_errors', true );


include "AgentsController.php";
include "SimulationsController.php";
include "UsersController.php";
include "PayoffFormulasController.php";

$instance = $_POST["class"];
$function = $_POST["function"];
$controller = new $instance;
$controller->$function($_POST);


?>