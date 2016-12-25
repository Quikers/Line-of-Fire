<?php

class Bootstrap {

    private $_url = null;
    private $_controller = null;
    
    private $_controllerPath = CONTROLLERS;
    private $_errorFile = CONTROLLERS . 'error.php';
    
    
    public function init()
    {
        // Sets the protected $_url
        $this->_getUrl();

        // Load the default controller if no URL is set
        // eg: Visit http://localhost it loads Default Controller
        if (empty($this->_url[0])) {
            $this->_loadDefaultController();
            return false;
        }

        $this->_loadExistingController();
        $this->_callControllerMethod();
    }
    
    private function _getUrl()
    {
        $url = isset($_GET['url']) ? $_GET['url'] : null;
        $url = rtrim($url, '/');
        $this->_url = explode('/', $url);
    }
    
    private function _loadDefaultController()
    {
        header("Location:" . URL . "home");
    }
    
    private function _loadExistingController()
    {
        $file = $this->_controllerPath . $this->_url[0] . '.php';
        
        if (file_exists($file)) {
            require $file;
            $this->_controller = new $this->_url[0];
        } else {
            $this->_error($this->_url[0]);
            return false;
        }
    }
    
    private function _callControllerMethod()
    {
        $length = count($this->_url);
        
        // Make sure the method we are calling exists
        if ($length > 1) {
            if (!method_exists($this->_controller, $this->_url[1])) {
                $this->_error($this->_url[1]);
            }
        }
        
        // Determine what to load
        switch ($length) {
            case 5:
                //Controller->Method(Param1, Param2, Param3, Param4)
                $this->_controller->{$this->_url[1]}(array($this->_url[2], $this->_url[3], $this->_url[4]));
                break;
            
            case 4:
                //Controller->Method(Param1, Param2, Param3)
                $this->_controller->{$this->_url[1]}(array($this->_url[2], $this->_url[3]));
                break;
            
            case 3:
                //Controller->Method(Param1, Param2)
                $this->_controller->{$this->_url[1]}(array($this->_url[2]));
                break;
            
            case 2:
                //Controller->Method(Param1)
                $this->_controller->{$this->_url[1]}();
                break;
            
            default:
                $this->_controller->index();
                break;
        }
    }
    
    private function _error($pageName) {
        require $this->_errorFile;
        $this->_controller = new WebsiteError($pageName);
        $this->_controller->index();
        
        exit;
    }
}