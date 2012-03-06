<?php
require_once 'MASGAUBase.php';
class Exporter extends MASGAUBase {

    public function __construct($file = null) {
        parent::__construct(1, 2, $file);
    }

    public function exportGameVersion($game,$version) {
        $new_game = $this->createElement("game");
        $new_game->appendChild($this->xml->createAttribute("name"))->
                appendChild($this->createTextNode($version->name));
        if ($version->platform != null) {
            $new_game->appendChild($this->xml->createAttribute("platform"))->
                    appendChild($this->createTextNode($version->platform));
        }
        if ($version->region != null) {
            $new_game->appendChild($this->xml->createAttribute("region"))->
                    appendChild($this->createTextNode($version->region));
        }
        if ($version->deprecated) {
            $new_game->appendChild($this->xml->createAttribute("deprecated"))->
                    appendChild($this->createTextNode("true"));
        }

        // Title
        if ($version->title!=null) {
            $new_game->appendChild($this->createElement("title",str_replace('&','&amp;'
                ,$version->title)));
        } else {
            $new_game->appendChild($this->createElement("title",str_replace('&','&amp;',$game->title)));
        }
        
        
        // Locations
        foreach($version->locations as $location) {
            $tag = null;
            switch(get_class($location)) {
                case "PathLocation":
                    $tag = $new_game->appendChild($this->createElement("location_path"));
                    $tag->appendChild($this->xml->createAttribute("environment_variable"))->
                            appendChild($this->createTextNode($location->ev));
                    $tag->appendChild($this->xml->createAttribute("path"))->
                            appendChild($this->createTextNode($location->path));
                    break;
                case "RegistryLocation":
                    $tag = $new_game->appendChild($this->createElement("location_registry"));
                    $tag->appendChild($this->xml->createAttribute("root"))->
                            appendChild($this->createTextNode($location->root));
                    $tag->appendChild($this->xml->createAttribute("key"))->
                            appendChild($this->createTextNode($location->key));
                    if ($location->value!=null) {
                        $tag->appendChild($this->xml->createAttribute("value"))->
                                appendChild($this->createTextNode($location->value));
                    }
                    break;
                case "ShortcutLocation":
                    $tag = $new_game->appendChild($this->createElement("location_shortcut"));
                    $tag->appendChild($this->xml->createAttribute("environment_variable"))->
                            appendChild($this->createTextNode($location->ev));
                    $tag->appendChild($this->xml->createAttribute("shortcut"))->
                            appendChild($this->createTextNode($location->shortcut));
                    break;
                case "GameLocation":
                    $tag = $new_game->appendChild($this->createElement("location_game"));
                    $tag->appendChild($this->xml->createAttribute("name"))->
                            appendChild($this->createTextNode($location->name));
                    if ($location->platform != null) {
                        $tag->appendChild($this->xml->createAttribute("platform"))->
                                appendChild($this->createTextNode($location->platform));
                    }
                    if ($location->region != null) {
                        $tag->appendChild($this->xml->createAttribute("country"))->
                                appendChild($this->createTextNode($location->region));
                    }
                    break;
                default:
                    throw new Exception("Location type ".get_class($location)." not known");
            }
            if ($location->append!=null) {
                $tag->appendChild($this->xml->createAttribute("append"))->
                        appendChild($this->createTextNode($location->append));
            }
            if ($location->detract!=null) {
                $tag->appendChild($this->xml->createAttribute("detract"))->
                        appendChild($this->createTextNode($location->detract));
            }
            if ($location->platform_version!=null) {
                $tag->appendChild($this->xml->createAttribute("platform_version"))->
                        appendChild($this->createTextNode($location->platform_version));
            }
            if ($location->deprecated) {
                $tag->appendChild($this->xml->createAttribute("deprecated"))->
                        appendChild($this->createTextNode("true"));
            }
        }         
         
        // Identifiers
        foreach ($version->files as $file) {
            if($file->mode=="IDENTIFIER") {
                $tag = $new_game->appendChild($this->createElement("identifier"));
                if($file->path!=null) {
                    $tag->appendChild($this->xml->createAttribute("path"))->
                            appendChild($this->createTextNode($file->path));
                }
                if($file->name!=null) {
                    $tag->appendChild($this->xml->createAttribute("filename"))->
                            appendChild($this->createTextNode($file->name));
                }
            }
        }
        // Saves/Ignores
        foreach ($version->files as $file) {
            switch($file->mode) {
                case "SAVE":
                    $tag = $new_game->appendChild($this->createElement("save"));
                    break;
                case "IGNORE":
                    $tag = $new_game->appendChild($this->createElement("ignore"));
                    break;
            }
            if($file->path!=null) {
                $tag->appendChild($this->xml->createAttribute("path"))->
                        appendChild($this->createTextNode($file->path));
            }
            if($file->name!=null) {
                $tag->appendChild($this->xml->createAttribute("filename"))->
                        appendChild($this->createTextNode($file->name));
            }
            if($file->type!=null) {
                $tag->appendChild($this->xml->createAttribute("type"))->
                        appendChild($this->createTextNode($file->type));
            }
            if($file->modified_after!=null) {
                $tag->appendChild($this->xml->createAttribute("modified_after"))->
                        appendChild($this->createTextNode(
                                date_format($file->modified_after,self::$date_format)));
            }
        }
        // PS Codes
        foreach ($version->ps_codes as $code) {
            $tag = $new_game->appendChild($this->createElement("ps_code"));
                $tag->appendChild($this->xml->createAttribute("prefix"))->
                        appendChild($this->createTextNode($code->prefix));
                $tag->appendChild($this->xml->createAttribute("suffix"))->
                        appendChild($this->createTextNode(str_pad($code->suffix, 5, "0", STR_PAD_LEFT)));
        }

        if ($version->override_virtualstore) {
            $tag = $new_game->appendChild($this->createElement("virtualstore"));
            $tag->appendChild($this->xml->createAttribute("override"))->
                    appendChild($this->createTextNode("true"));
        }
        if ($version->require_detection) {
            $new_game->appendChild($this->createElement("require_detection"));
        }


        // Write comments
        if ($version->comment != null) {
            $new_game->appendChild($this->createElement("comment", $version->comment));
        }
        if ($version->restore_comment != null) {
            $new_game->appendChild($this->createElement("restore_comment", $version->restore_comment));
        }

        // Contributors
        foreach ($version->contributors as $contributor) {
            $new_game->appendChild($this->createElement("contributor", $contributor));
        }
        return $new_game;
    }

}

?>
