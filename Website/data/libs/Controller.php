<?php

class Controller {

    public $model = NULL;
    
    function __construct() {    
        session::init();
        
        $this->view = new View();
    }
    
    public function loadModel($name, $modelPath = NULL) {
        $modelPath = ($modelPath == NULL ? str_replace("\\", "/", __DIR__) . "/../models/" : $modelPath);
        $path = $modelPath . $name.'Model.php';
        
        if (file_exists($path)) {
            require $path;
            
            $name .= 'Model';
            $this->model = new $name();
        }
    }
}