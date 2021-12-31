<?php
namespace CoreController;

class core
{

    public $con;

    public function __construct()
    {
        $this->con = mysqli_connect("localhost", "root", "root", "unityaccess");

        //check that connection happen
        if(mysqli_connect_error())
        {
            echo "1; Connection failed";
            exit();
        }
    }

}

?>