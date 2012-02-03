<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
 
/**
 * Description of GameVersion
 *
 * @author TKMAYN9
 */
class GameVersion {
    // Tag properties
    public $title = null;
    public $name;
    public $platform = null;
    public $region = null;
    public $deprecated = false;
    
    public $override_virtualstore = false;
    public $require_detection = false;

    public $comment = null;
    public $restore_comment = null;

    public $locations = array();
    public $files = array();
    public $contributors = array();
    public $ps_codes = array();

    public function loadFromDb($id) {
        $sql = 'select * from masgau_game_data.game_versions where id = '.$id.'';
        $result = mysql_query($sql);
        
        if($row = mysql_fetch_assoc($result)) {
            $this->name = $row['name'];
            if($row['platform']!='UNIVERSAL')
                $this->platform = $row['platform'];
            
            if($row['region']!='UNIVERSAL')
                $this->region = $row['region'];
            
            $this->deprecated = $row['deprecated'];
            
            $this->title = $row['version_title'];
            
            $this->override_virtualstore = $row['override_virtualstore'];
            $this->require_detection = $row['require_detection'];
            
            $this->comment = $row['comment'];
            $this->restore_comment = $row['restore_comment'];
            
            // Load paths
            $sql = 'select * from masgau_game_data.game_locations loc, 
                masgau_game_data.game_paths path where game_version = '.$id.' and loc.id = path.id';
            $result = mysql_query($sql);
            while($row = mysql_fetch_assoc($result)) {
                require_once 'PathLocation.php';
                $loc = new PathLocation();
                $loc->loadfromDb($row['id']);
                
                $c = sizeof($this->locations);
                $this->locations[$c] = $loc;
            }
            
            // Load registry keys
            $sql = 'select * from masgau_game_data.game_locations loc, 
                masgau_game_data.game_registry_keys `keys` where game_version = '.$id.' and loc.id = `keys`.id';
            if($result = mysql_query($sql)) {
                while($row = mysql_fetch_assoc($result)) {
                    require_once 'RegistryLocation.php';
                    $loc = new RegistryLocation();
                    $loc->loadfromDb($row['id']);

                    $c = sizeof($this->locations);
                    $this->locations[$c] = $loc;
                }
            }

            // Load shortcuts
            $sql = 'select * from masgau_game_data.game_locations loc, 
                masgau_game_data.game_shortcuts short where game_version = '.$id.' and loc.id = short.id';
            if($result = mysql_query($sql)) {
                while($row = mysql_fetch_assoc($result)) {
                    require_once 'ShortcutLocation.php';
                    $loc = new ShortcutLocation();
                    $loc->loadfromDb($row['id']);

                    $c = sizeof($this->locations);
                    $this->locations[$c] = $loc;
                }
            }
            
            // Load parents
            $sql = 'select * from masgau_game_data.game_locations loc, 
                masgau_game_data.game_parents par where game_version = '.$id.' and loc.id = par.id';
            if($result = mysql_query($sql)) {
                while($row = mysql_fetch_assoc($result)) {
                    require_once 'GameLocation.php';
                    $loc = new GameLocation();
                    $loc->loadfromDb($row['id']);

                    $c = sizeof($this->locations);
                    $this->locations[$c] = $loc;
                }
            }
            
            
            // Load playstation codes
            $sql = 'select * from masgau_game_data.playstation_codes where game_version = '.$id.'';
            $result = mysql_query($sql);
            while($row = mysql_fetch_assoc($result)) {
                require_once 'PlayStationCode.php';
                $code = new PlayStationCode();
                $code->loadFromDb($row);
                $c = sizeof($this->ps_codes);
                $this->ps_codes[$c] = $code;
            }
            
            // Load files
            $sql = 'select * from masgau_game_data.files where game_version = '.$id.'';
            $result = mysql_query($sql);
            while($row = mysql_fetch_assoc($result)) {
                require_once 'SaveFile.php';
                $code = new SaveFile();
                $code->loadFromDb($row);
                $c = sizeof($this->files);
                $this->files[$c] = $code;
            }
            
             
            // Load contributors
            $sql = 'select * from masgau_game_data.contributions where game_version = '.$id.'';
            $result = mysql_query($sql);
            while($row = mysql_fetch_assoc($result)) {
                $c = sizeof($this->contributors);
                $this->contributors[$c] = $row['contributor'];
            }
        }        
    }
    
    function loadFromXml($node) {
        global $wgOut;
        foreach ($node->attributes as $attribute) {
            switch ($attribute->name) {
                case 'name':
                    $this->name = $attribute->value;
                    break;
                case 'platform':
                    $this->platform = $attribute->value;
                    break;
                case 'country':
                case 'region':
                    $this->region = $attribute->value;
                    break;
                case 'deprecated':
                    if($attribute->value=="true")
                        $this->deprecated = true;
                    break;
                default:
                    throw new Exception($attribute->name . ' not supported');
            }
        }

        $wgOut->addHTML('<table class="wikitable">');

        foreach ($node->childNodes as $element) {
            $l = sizeof($this->locations);
            $f = sizeof($this->files);
            $c = sizeof($this->contributors);
            $p = sizeof($this->ps_codes);
            switch ($element->localName) {
                case '':
                    break;
                case 'title':
                    $this->title = $element->textContent;
                    break;
                case 'location_registry':
                    require_once 'RegistryLocation.php';
                    $loc = new RegistryLocation();
                    $loc->loadFromXml($element);
                    $this->locations[$l] = $loc;
                    break;
                case 'location_path':
                    require_once 'PathLocation.php';
                    $loc = new PathLocation();
                    $loc->loadFromXml($element);
                    $this->locations[$l] = $loc;
                    break;
                case 'location_shortcut':
                    require_once 'ShortcutLocation.php';
                    $loc = new ShortcutLocation();
                    $loc->loadFromXml($element);
                    $this->locations[$l] = $loc;
                    break;
                case 'location_game':
                    require_once 'GameLocation.php';
                    $loc = new GameLocation();
                    $loc->loadFromXml($element);
                    $this->locations[$l] = $loc;
                    break;
                case 'save':
                case 'ignore':
                case 'identifier':
                    require_once 'SaveFile.php';
                    $file = new SaveFile();
                    $file->loadFromXml($element);
                    $this->files[$f] = $file;
                    break;
                case 'ps_code':
                    require_once 'PlayStationCode.php';
                    $code = new PlayStationCode();
                    $code->loadFromXml($element);
                    $this->ps_codes[$p] = $code;
                    break;
                case 'contributor':
                    $this->contributors[$c] = $element->textContent;
                    break;
                case 'comment':
                    $this->comment = $element->textContent;
                    break;
                case 'restore_comment':
                    $this->restore_comment = $element->textContent;
                    break;
                case 'virtualstore':
                    switch ($element->attributes->getNamedItem('override')->value) {
                        case "yes":
                        case "true":
                            $this->override_virtualstore = true;
                            break;
                                
                    }
                case 'require_detection':
                    $this->require_detection = true;
                    break;
                default:
                    throw new Exception($element->localName . ' not supported');
            }
        }


        $wgOut->addHTML('<tr><td>VStoreOverride: ' . $this->override_virtualstore . '</td></tr>');
        $wgOut->addHTML('<tr><td>Title: ' . $this->title . '</td></tr>');
        $wgOut->addHTML('<tr><td>Version: ' . $this->getVersionString() . '</td></tr>');

        if ($this->require_detection) {
            $wgOut->addHTML('<tr><td>Requires Detection For Restore</td></tr>');
        }

        foreach ($this->contributors as $comment) {
            $wgOut->addHTML('<tr><td>Contrib: ' . $comment . '</td></tr>');
        }

        $wgOut->addHTML('<tr><td>Comment: ' . $this->comment . '</td></tr>');
        $wgOut->addHTML('<tr><td>Restore Comment: ' . $this->restore_comment . '</td></tr>');

        if ($this->deprecated) {
            $wgOut->addHTML('<tr><td>GAME IS DEPRECATED</td></tr>');
        }

        $wgOut->addHTML('</table>');
    }


    public function writeToDb($replace) {
        global $wgOut;

        $dbr = wfGetDB(DB_SLAVE);
        $criteria = array();
        $criteria[0] = 'name = \'' . $this->name . '\'';

        $insert = array('name' => $this->name);

        if($this->comment!=null)
            $insert['comment'] = $this->comment;
        if($this->restore_comment!=null)
            $insert['restore_comment'] = $this->restore_comment;
        
        if($this->deprecated)
            $insert['deprecated'] = $this->deprecated;
        if($this->override_virtualstore)
            $insert['override_virtualstore'] = $this->override_virtualstore;
        if($this->require_detection)
            $insert['require_detection'] = $this->require_detection;

        if ($this->platform == null) {
            $criteria[1] = 'platform = \'UNIVERSAL\'';
        } else {
            $insert['platform'] = $this->platform;
            $criteria[1] = 'platform = \'' . $this->platform . '\'';
        }

        if ($this->region == null) {
            $criteria[2] = 'region = \'UNIVERSAL\'';
        } else {
            $insert['region'] = $this->region;
            $criteria[2] = 'region = \'' . $this->region . '\'';
        }

        $res = $dbr->select('masgau_game_data.game_versions', 'count(*) as count', // $vars (columns of the table)
                        $criteria, __METHOD__, // $fname = 'Database::select',
                        null
                    );

        $dbw = wfGetDB(DB_MASTER);

        if ($res->fetchObject()->count==1) {
            $wgOut->addWikiText('** \'\'\'Game version ' . $this->getVersionString() . ' already exists in database\'\'\'');
            if (!$replace) {
                $wgOut->addWikiText('*** \'\'\'Replace is not enabled, skipping\'\'\'');
                return;
            }
            $wgOut->addWikiText('*** \'\'\'Replace enabled, deleting current version\'\'\'');
            
            $res = $dbw->select('masgau_game_data.game_versions', 'id', // $vars (columns of the table)
                            $criteria, __METHOD__, // $fname = 'Database::select',
                            SQL_NO_CACHE
                        );

            $id = $res->fetchObject()->id;
            $insert['id'] = $id;

            $dbw->delete('masgau_game_data.game_versions',$criteria, $fname= 'DatabaseBase::delete');
        }

        $res = $dbr->select('masgau_game_data.games', 'title', // $vars (columns of the table)
                        'name = \'' . $this->name . '\'', __METHOD__, // $fname = 'Database::select',
                        null
                    );
        if ($this->title != null && $res->fetchObject()->title != $this->title) {
            $insert['version_title'] = $this->title;
        }

        $wgOut->addWikiText('** Writing game version ' . $this->getVersionString() . ' to database (' . $this->name . ')');

        $dbw->insert('masgau_game_data.game_versions', $insert, $fname = 'Database::insert', $options = array());

        // Gets the Id of the new row
        $res = $dbw->select('masgau_game_data.game_versions', 'id', // $vars (columns of the table)
                        $criteria, __METHOD__, // $fname = 'Database::select',
                        SQL_NO_CACHE
                    );

        $id = $res->fetchObject()->id;
        
        foreach($this->contributors as $contributor) {
            $wgOut->addWikiText('** Writing contribution by ' . $contributor . ' to database');
            
            $res = $dbr->select('masgau_game_data.contributors', 'count(*) count', // $vars (columns of the table)
                            'name = \''.$contributor.'\'', __METHOD__, // $fname = 'Database::select',
                            null
                        );
            if($res->fetchObject()->count==0) {
                $wgOut->addWikiText('*** Contributor is new, adding');
                $dbw->insert('masgau_game_data.contributors', array('name'=>$contributor), 
                        $fname = 'Database::insert', $options = array());
            }
            
            $dbw->insert('masgau_game_data.contributions', 
                    array('game_version'=>$id,
                        'contributor'=>$contributor), 
                    $fname = 'Database::insert', $options = array());
            
        }
        foreach($this->ps_codes as $ps_code) {
            $ps_code->writeToDb($id);
        }
        foreach($this->files as $file) {
            $file->writeToDb($id);
        }
        foreach($this->locations as $location) {
            $location->writeToDb($id);
        }
    }

    public function getVersionString() {
        $return_me = $this->name;

        if ($this->platform != null)
            $return_me .= '>' . $this->platform;

        if ($this->region != null)
            $return_me .= '>' . $this->region;

        return $return_me;
    }

}

?>
