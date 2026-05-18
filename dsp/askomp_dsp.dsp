declare name 		"Askomp";
declare version 	"1.0";
declare author 		"DSP: UnD3ath, Plugin: Sout";
declare copyright 	"Aqua Sounds";

import("stdfaust.lib");

pos(x) = max(x, 0);
neg(x) = min(x, 0);

compress(l,r) = pos(l),pos(r),neg(l),neg(r):
co.FFcompressor_N_chan(1 - (1 / ratio),postive_threshold,atk*0.001,rel*0.001,3,0,0,_,2),
co.FFcompressor_N_chan(1 - (1 / ratio),negative_threshold,atk*0.001,rel*0.001,3,0,0,_,2):>
_,_; 

postive_threshold = vslider("Positive Threshold",0,-36,0,0.1);
negative_threshold = vslider("Negative Threshold",0,-36,0,0.1);
atk = hslider("Attack",10,0,50,0.1);
rel = hslider("Release",50,20,200,1);
ratio = hslider("Raio",2,1,20,0.1);
trim = hslider("Trim",0,-9,9,0.1);

process(l,r) = compress(l,r) : 
_*ba.db2linear(trim),_*ba.db2linear(trim);