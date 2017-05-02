<?php
    include_once("../header.php");
?>
<div class='row' style='margin-top:70px;'>
<ul class='nav nav-tabs'>
<li class='active'><a href='#'>categoria</a></li>
<li><a href='../cliente/cliente.php'>cliente</a></li>
</ul>
</div>
<a href='addcategoria.php' class='btn btn-success' style='margin-top: 40px; padding-left: 39px; padding-right: 39px;'>Añadir categoria</a>
<div class='row' style='margin-top:30px;'>
<div class='table-responsive'>
<table class='table'>
<thead>
<tr>
<th>Nombre</th>
<th>descripcion</th>
</tr>
</thead>
<tbody>
<?php
$resultSet = $db->squery_rows('SELECT nombre,descripcion FROM categoria WHERE id NOT IN (2,3)', array());
while($data = mysqli_fetch_row($resultSet)){
echo '<tr>';
foreach($data as $col){
echo '<td>'.$col.'</td>';}
echo '</tr>';}
?>
</tbody>
</table>
</div>
</div>
<?php
    include_once("../footer.php");
?>
