#!/usr/bin/perl

use XML::Simple;
use Data::Dumper;

$debug = false;
$target = "library";
for ($i=0; $i <= $#ARGV; $i++) {
	print $i.":".$ARGV[$i]."\n";
	if($ARGV[$i] eq "-debug") {
		$debug = true;
	}
	if($ARGV[$i] eq "-target") {
		$i++;
		$target = $ARGV[$i];
	}
	if($ARGV[$i] eq "-name") {
		$i++;
		$name = $ARGV[$i];	
	}
}

if (defined $name) {
	print "\nCOMPILING ".$name."\n";
} else {
	print $name;
	print "PLEASE USE -name TO PROVIDE THE NAME OF WHATEVER TO COMPILE\n";
	exit 1;
}


$xml  = new XML::Simple (ForceArray => 1);
$data = $xml->XMLin($name."/".$name.".csproj");

$references = "";

foreach $e (@{$data->{ItemGroup}}) {
	if (defined $e->{Reference}) {
		foreach $f (@{$e->{Reference}}) {
			@whut = split(/,/,$f->{Include});
			$references = $references." -r:".$whut[0];
		}
	}
	if (defined $e->{ProjectReference}) {
		foreach $f (@{$e->{ProjectReference}}) {
			$whut = $f->{Name}[0];
			$references = $references." -r:build/".$whut.".dll";
		}
		
	}
}

$command = "dmcs";
if($debug) {
	$command .= " -debug";
}
$command .= " -target:".$target;
$command .= $references;
$command .= " -recurse:".$name."/*.cs";
$command .= " -out:build/".$name.".dll";

`export MONO_GAC_PREFIX=/usr/lib/cli:$MONO_GAC_PREFIX`;
`mkdir build`;
print $command."\n";

system ($command)==0
	or die "Did NOT compile properly!";