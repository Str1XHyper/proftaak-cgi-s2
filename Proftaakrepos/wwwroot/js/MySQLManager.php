<?php echo
    $servername = "185.182.57.161";
    $username = "tijnvcd415_Proftaak";
    $password = "Proftaak";
    $dbname = "tijnvcd415_Proftaak";

    // Create connection
    $conn = new mysqli($servername, $username, $password, $dbname);
    // Check connection
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    } 

    $sql = "SELECT * FROM Events";
    $result = $conn->query($sql);
    echo json_encode($result);
?>