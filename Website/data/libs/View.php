<?php

class View {

    function __construct() {
        
    }

    public function render($name, $noInclude = false)
    {
        if ($noInclude != true) { require 'data/views/header.php'; }
        require 'data/views/' . $name . '.php';
        if ($noInclude != true) { require 'data/views/footer.php'; }
    }

}