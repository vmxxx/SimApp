<?php
namespace AgentsController;
include_once "CoreController.php";
use CoreController\core as core;

class agents extends core
{
    public function create($data)
    {
		$icon = $data["icon"];
		$name = $data["name"];
		$description = $data["description"];
		$authorID = $data["authorID"];
        $namecheckquery = "INSERT INTO agents (icon, name, description, authorID) VALUES (\"$icon\", \"$name\", \"$description\", $authorID);";
		$this -> con -> query ($namecheckquery);
		echo "0;";
    }

    public function read($data)
    {
		if ($data["fullList"] === "false")
		{
			$IDsArray = $data["IDsArray"];
			$SQL = "SELECT * FROM agents WHERE ID IN ($IDsArray)";
			$result = $this -> con -> query ($SQL) or die("sql failed!");
			echo '0;'.$result->num_rows;
			for($i = 0; $i < $result->num_rows; $i++)
			{
				$row = $result -> fetch_assoc();
				echo'{ID:'.$row["ID"].', icon:'.$row["icon"].', name:'.$row["name"].', description:"'.$row["description"].'", authorID:'.$row["authorID"].'}';
			}
			/**/
		}
		else //if ($data["fullList"] === "true")
		{
			$authorID = $data["authorID"];

			$namecheckquery = 'SELECT * FROM agents WHERE authorID = '.$authorID.' OR authorID = 1;';

			$result = $this -> con -> query ($namecheckquery);
			echo '0;'.$result->num_rows;
			for($i = 0; $i < $result->num_rows; $i++)
			{
				$row = $result -> fetch_assoc();
				echo'{ID:'.$row["ID"].', icon:"'.$row["icon"].'", name:"'.$row["name"].'", description:"'.$row["description"].'", authorID:'.$row["authorID"].'}';
			}
		}
    }

    public function update($data)
    {
		$ID = $data["ID"];
		$icon = $data["icon"];
		$name = $data["name"];
		$description = $data["description"];
		$authorID = $data["authorID"];
		
        $namecheckquery = "UPDATE agents SET icon = \"$icon\", name = \"$name\", description = \"$description\" WHERE agents.ID = $ID;";
		$this -> con -> query ($namecheckquery);
		echo "0;";
		
    }

    public function delete($data)
    {
		$ID = $data["ID"];
		$namecheckquery = "DELETE FROM agents WHERE agents.ID = $ID AND agents.authorID != 0;";
		$this -> con -> query ($namecheckquery);
		echo "0;";
    }

}
?>