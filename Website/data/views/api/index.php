<link rel="stylesheet" href="<?= URL ?>public/css/api.css">

<div id="cluster">
    <aside>
        <div id="api-nav">
            <a class="header" href="<?= URL ?>api/docs">Documentation Home</a>
            
            <?php
            
            $content = array();
            
            foreach($this->methodGroups as $groupName) {
                $content[$groupName] = array();
                
                echo '<a class="header1-link" href="#' . $groupName . '">' . $groupName . '</a>';
                foreach(get_class_vars($groupName) as $varName => $description) {
                    $content[$groupName][$varName] = $description;
                    echo '<a class="header2-link" href="#' . $varName . '">' . $varName . '</a>';
                }
            }
            
            ?>
            
        </div>
    </aside>

    <div id="api-content">
        <h1>API Documentation</h1>
        <p>If any of the API requests fail, the returned value is the following:</p><pre>false</pre>
        <?php
        
        $i = 0;
            
        foreach($content as $groupName => $group) {
            if ($i++ != 0) {
                echo '<div class="separator"></div>';
            }
            
            echo '<a id="' . $groupName . '" class="group jumpTo">' . $groupName . '</a><br>';
            foreach($content[$groupName] as $methodName => $description) {
                echo '<a id="' . $methodName . '" class="method jumpTo">' . $methodName . '</a><br>'
                        . $description;
            }
        }
        
        ?>
    </div>
</div>