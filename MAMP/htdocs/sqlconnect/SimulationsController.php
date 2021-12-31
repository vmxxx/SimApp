<?php
namespace SimulationsController;
include_once "CoreController.php";
use CoreController\core as core;

class simulations extends core
{
    public function create($data)
    {
		$name = $data["name"];
		$image = $data["image"];
		$description = $data["description"];
		$authorID = $data["authorID"];
        $SQL = "INSERT INTO simulations (name, image, description, likesCount, dislikeCount, authorID) VALUES (\"$name\", \"$image\", \"$description\", 0, 0, $authorID);";
        //$namecheckquery = "INSERT INTO simulations (name, image, description, likesCount, dislikeCount, authorID) VALUES (\"name\", \"image\", \"description\", 0, 0, 5);";
        //$namecheckquery = "INSERT INTO agents (icon, name, description, authorID) VALUES (\"icon\", \"name\", \"description\", 5);";
		
		/*
		$result = $this -> con -> query ($SQL) or die("sql failed!");
		$last_id = $this -> con -> insert_id;
		echo "0; $last_id";
		/**/
		//$result = $this -> con -> query ($SQL);
		$result = $this -> con -> query ($SQL) or die("sql failed!");
		$last_id = $this -> con -> insert_id;
		echo "0; $last_id; $SQL";
		//echo "0;";
			
			
			/*
		if ($result != "sql failed!") 
		{
			$last_id = $this -> con -> insert_id;
			echo "0; $last_id";
		} 
		else {
			echo "Error: " . $SQL . "<br>" . $this -> con -> error;
		}
		/**/
		/*
		if ($this -> con -> query ($SQL) === TRUE) 
		{
			$last_id = $this -> con -> insert_id;
			echo "0; $last_id";
		} 
		else {
			echo "Error: " . $SQL . "<br>" . $this -> con -> error;
		}
		/**/
		/*
		
		/*
		for($i = 1; $i <= $data["agentCount"]; $i++)
		{
			for($j = 1; $j <= $data["agentCount"]; $j++)
			{
				$agent1 = $data[$i."_".$j."_payoffFormula_agent1"];
				$agent2 = $data[$i."_".$j."_payoffFormula_agent2"];
				$payoffFormula = $data[$i."_".$j."_payoffFormula_payoffFormula"];
				$simulationID = $data[$i."_".$j."_payoffFormula_simulationID"];
				
				$SQL = "INSERT INTO payoffFormulas (agent1, agent2, payoffFormula, authorID) VALUES ($agent1, $agent2, \"$payoffFormula\", $simulationID);";
				
				$this -> con -> query ($SQL);
			}
		}
		/**/
		
    }

    public function read($data)
    {	
		if ($data["scene"] == "creation")
		{
			$ID = $data["ID"];
			$SQL = "SELECT * FROM simulations WHERE ID = $ID;";
			$result = $this -> con -> query ($SQL);
			
            $existinginfo = mysqli_fetch_assoc($result);
			echo'0;{ID:'.$existinginfo["ID"].', name:"'.$existinginfo["name"].'", image:"'.$existinginfo["image"].'", description:"'.$existinginfo["description"].'", likesCount:'.$existinginfo["likesCount"].', dislikesCount:'.$existinginfo["dislikeCount"].', authorID:'.$existinginfo["authorID"].'}';

		}
		else if ($data["list"] == "popular")
		{
			$N = $data["N"];
			$LIMIT = 10 * $N;
			$SQL = 'SELECT * FROM simulations LIMIT '.$LIMIT.';';

			$result = $this -> con -> query ($SQL);
			echo '0;'.$result->num_rows.'{';
			for($i = 0; $i < $result->num_rows; $i++)
			{
				$row = $result -> fetch_assoc();
				echo'{ID:'.$row["ID"].', name:"'.$row["name"].'", image:"'.$row["image"].'", description:"'.$row["description"].'", likesCount:'.$row["likesCount"].', dislikesCount:'.$row["dislikeCount"].', authorID:'.$row["authorID"].'}';
			}
			echo '}0;';
		}
		else //if ($data["list"] == "user")
		{
			$N = $data["N"];
			$LIMIT = 10 * $N;
			$authorID = $data["authorID"];
			
			$SQL = 'SELECT * FROM simulations WHERE authorID = '.$authorID.' LIMIT '.$LIMIT.';';
			$result = $this -> con -> query ($SQL);
			echo '0;'.$result->num_rows.'{';
			for($i = 0; $i < $result->num_rows; $i++)
			{
				$row = $result -> fetch_assoc();
				echo'{ID:'.$row["ID"].', name:"'.$row["name"].'", image:"'.$row["image"].'", description:"'.$row["description"].'", likesCount:'.$row["likesCount"].', dislikesCount:'.$row["dislikeCount"].', authorID:'.$row["authorID"].'}';
			}
			echo '}0;';
		}
    }

    public function update()
    {
		/*
		$icon = $data["ID"];
		$name = $data["name"];
		$name = $data["image"];
		$description = $data["description"];
		$description = $data["likesCount"];
		$description = $data["dislikesCount"];
		$authorID = $data["authorID"];
        $namecheckquery = "INSERT INTO simulations (name, image, description, likesCount, dislikesCount, authorID) VALUES (\"$name\", \"$image\", \"$description\", 0, 0, $authorID);";
		
        $namecheckquery2 = "UPDATE simulations SET name = \"$name\", image = \"$iamge\", description = \"$description\", likesCount = 0, dislikesCount = 0 WHERE simulations.ID = $ID;";
		$this -> con -> query ($namecheckquery);
		$this -> con -> query ($namecheckquery2);
		echo "0;";
		/**/
    }

    public function delete($data)
    {
		
		$ID = $data["ID"];
		$SQL = "DELETE FROM simulations WHERE simulations.ID = $ID;";
		$this -> con -> query ($SQL);
		echo "0;";
		/**/
    }

}
?>