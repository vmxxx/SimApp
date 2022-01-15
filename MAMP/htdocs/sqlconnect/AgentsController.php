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
        $SQL = "INSERT INTO agents (icon, name, description, authorID) VALUES (\"$icon\", \"$name\", \"$description\", $authorID);";
		$this -> con -> query ($SQL);
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
			if($data["onSearch"] == "false")
			{
				$authorID = $data["authorID"];

				$SQL = 'SELECT * FROM agents WHERE authorID = '.$authorID.' OR authorID = 1;';

				$result = $this -> con -> query ($SQL);
				echo '0;'.$result->num_rows;
				for($i = 0; $i < $result->num_rows; $i++)
				{
					$row = $result -> fetch_assoc();
					echo'{ID:'.$row["ID"].', icon:"'.$row["icon"].'", name:"'.$row["name"].'", description:"'.$row["description"].'", authorID:'.$row["authorID"].'}';
				}
			}
			else
			{
				$authorID = $data["authorID"];
				$search = $data["search"];

				$SQL = "SELECT * FROM agents WHERE name LIKE '%$search%' AND(authorID = $authorID OR authorID = 1) LIMIT 10;";

				$result = $this -> con -> query ($SQL);
				echo '0;'.$result->num_rows;
				for($i = 0; $i < $result->num_rows; $i++)
				{
					$row = $result -> fetch_assoc();
					echo'{ID:'.$row["ID"].', icon:"'.$row["icon"].'", name:"'.$row["name"].'", description:"'.$row["description"].'", authorID:'.$row["authorID"].'}';
				}
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
		
        $SQL = "UPDATE agents SET icon = \"$icon\", name = \"$name\", description = \"$description\" WHERE agents.ID = $ID;";
		$this -> con -> query ($SQL);
		echo "0;";
		
    }

    public function delete($data)
    {
		$ID = $data["ID"];
        $SQL = "SELECT * FROM payoffFormulas WHERE agent1 = $ID;";
        //check if name exists
        $result = mysqli_query($this->con, $SQL);

        if(mysqli_num_rows($result) > 0)
        {
            echo "Can't delete agent! Such agent is inside some simulation!";
        }
		else
		{
			$SQL = "DELETE FROM agents WHERE agents.ID = $ID AND agents.authorID != 0;";
			$this -> con -> query ($SQL);
			echo "0; Agent deleted successfully!";
		}
    }

}
?>