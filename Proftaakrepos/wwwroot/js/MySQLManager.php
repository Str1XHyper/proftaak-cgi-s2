<?php echo

    $servername = "185.182.57.161";
    $username = "tijnvcd415_Proftaak";
    $password = "Proftaak";
    $dbname = "tijnvcd415_Proftaak";
$connect = new PDO($servername, $username, $password, $dbname);

$data = array();

$query = "SELECT * FROM Rooster ORDER BY EventId";

$statement = $connect->prepare($query);

$statement->execute();

$result = $statement->fetchAll();

foreach($result as $row)
{
 $data[] = array(
  'id'   => $row["EventId"],
  'title'   => $row["Subject"],
  'start'   => $row["Start"],
  'end'   => $row["End"]
 );
}

echo json_encode($data);

?>