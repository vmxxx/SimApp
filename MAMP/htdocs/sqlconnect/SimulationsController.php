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
        $SQL = "INSERT INTO simulations (name, image, description, likesCount, dislikeCount, authorID, approved) VALUES (\"$name\", \"$image\", \"$description\", 0, 0, $authorID, 0);";
		
		echo $image;
		$result = $this -> con -> query ($SQL);
		$last_id = $this -> con -> insert_id;
		echo "0; $last_id";
		
    }

    public function read($data)
    {	
		if ($data["scene"] == "creation")
		{
			$ID = $data["ID"];
			$SQL = "SELECT * FROM simulations WHERE ID = $ID;";
			$result = $this -> con -> query ($SQL);
			
            $existinginfo = mysqli_fetch_assoc($result);
			echo'0;{ID:'.$existinginfo["ID"].', name:"'.$existinginfo["name"].'", image:"'.$existinginfo["image"].'", description:"'.$existinginfo["description"].'", likesCount:'.$existinginfo["likesCount"].', dislikesCount:'.$existinginfo["dislikeCount"].', approved:'.$existinginfo["approved"].', authorID:'.$existinginfo["authorID"].'}';

		}
		else if ($data["list"] == "popular")
		{
			if($data["onSearch"] == "true")
			{
				$search = $data["search"];
				$SQL = "SELECT * FROM simulations WHERE name LIKE '%$search%' LIMIT 10;";
				
				$result = $this -> con -> query ($SQL);
				echo '0;'.$result->num_rows.'{';
				for($i = 0; $i < $result->num_rows; $i++)
				{
					$row = $result -> fetch_assoc();
					echo'{ID:'.$row["ID"].', name:"'.$row["name"].'", image:"'.$row["image"].'", description:"'.$row["description"].'", likesCount:'.$row["likesCount"].', dislikesCount:'.$row["dislikeCount"].', approved:'.$row["approved"].', authorID:'.$row["authorID"].'}';
				}
				echo '}0;';
			}
			else
			{
				$N = $data["N"];
				$LIMIT = 10 * $N;
				$SQL = 'SELECT * FROM simulations ORDER BY likesCount DESC LIMIT '.$LIMIT.';';

				$result = $this -> con -> query ($SQL);
				echo '0;'.$result->num_rows.'{';
				for($i = 0; $i < $result->num_rows; $i++)
				{
					$row = $result -> fetch_assoc();
					echo'{ID:'.$row["ID"].', name:"'.$row["name"].'", image:"'.$row["image"].'", description:"'.$row["description"].'", likesCount:'.$row["likesCount"].', dislikesCount:'.$row["dislikeCount"].', approved:'.$row["approved"].', authorID:'.$row["authorID"].'}';
				}
				echo '}0;';
			}
		}
		else //if ($data["list"] == "user")
		{
			if($data["onSearch"] == "true")
			{
				$authorID = $data["authorID"];
				$search = $data["search"];
				$SQL = "SELECT * FROM simulations WHERE authorID = $authorID AND name LIKE '%$search%' LIMIT 10;";
				
				$result = $this -> con -> query ($SQL);
				echo '0;'.$result->num_rows.'{';
				for($i = 0; $i < $result->num_rows; $i++)
				{
					$row = $result -> fetch_assoc();
					echo'{ID:'.$row["ID"].', name:"'.$row["name"].'", image:"'.$row["image"].'", description:"'.$row["description"].'", likesCount:'.$row["likesCount"].', dislikesCount:'.$row["dislikeCount"].', approved:'.$row["approved"].', authorID:'.$row["authorID"].'}';
				}
				echo '}0;';
			}
			else
			{
				$N = $data["N"];
				$LIMIT = 10 * $N;
				$authorID = $data["authorID"];
				
				$SQL = 'SELECT * FROM simulations WHERE authorID = '.$authorID.' ORDER BY likesCount DESC LIMIT '.$LIMIT.';';
				$result = $this -> con -> query ($SQL);
				echo '0;'.$result->num_rows.'{';
				for($i = 0; $i < $result->num_rows; $i++)
				{
					$row = $result -> fetch_assoc();
					echo'{ID:'.$row["ID"].', name:"'.$row["name"].'", image:"'.$row["image"].'", description:"'.$row["description"].'", likesCount:'.$row["likesCount"].', dislikesCount:'.$row["dislikeCount"].', approved:'.$row["approved"].', authorID:'.$row["authorID"].'}';
				}
				echo '}0;';
			}
		}
    }

    public function update($data)
    {
		if($data["onApproval"] === "true")
		{
			$ID = $data["ID"];
			$approved = $data["approved"];
			$SQL = "UPDATE simulations SET approved = $approved WHERE ID = $ID";
			$result = $this -> con -> query ($SQL) or die("sql failed!");
			echo "0; Simulation approved successfully!";
		}
		else
		{
			$ID = $data["ID"];
			$name = $data["name"];
			$image = $data["image"];
			$description = $data["description"];
			
			$SQL = "UPDATE simulations SET name = \"$name\", image = \"$image\", description = \"$description\" WHERE ID = $ID";
			
			$result = $this -> con -> query ($SQL) or die("sql failed!");
			echo "0; $ID";
			//echo "0; $ID; $SQL";
		}
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