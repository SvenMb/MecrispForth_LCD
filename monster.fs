( start monster: ) here dup hex.

\ original invader from atari as 3 chars
\ load with LCD_ram
\ load: invader $40 LCD_ram
\ use:  0 LCD_send 1 LCD_send 2 LCD_send
create invader
hex
04
02
07
0D h ,
1F
15
14
03 h ,

02
04
1E
1B h ,
1F
1A
02
0C h ,

00
00
00
00 h ,
10
10
10
00 h ,

FFFF
,
decimal

\ half monster only 8 bytes allowed for loading with LCD_monster
\ just for fun
create monster
hex
04
02
07
0D h ,
1F
15
14
03 h
,
decimal

create monster2
hex
01
03
07
0D h ,
0F
02
05
0A h
,
decimal

\ loader for above symetric Monster in 3 chars
\ with possibility to move it pixelwise
\ see monster_demo below
\ load: monster 0 $40
\ load with 1 pixel shift: monster 1 $40 LCD_monster
: LCD_monster ( addr shift start - - )
    over 4 > if 2drop drop exit then \ bad shift
    dup $40 < if 2drop drop exit then \ bad addr
    lcd_cmd
    8 0 do
	over i + c@
	0
	2 pick negate 5 + 0 do
	    i 10 > if exit then
	    over 4 i - bit and
	    0<> if
		2 pick i + negate 4 + bit or
	    then
	loop
	lcd_send
	drop
    loop

    dup 0<> if
	8 0 do
	    over i + c@
	    0
	    2 pick 1+ 1 do
		over 3 pick i - bit and
		0<> if
		    5 i - bit  or
		then
	    loop
	    
	    2 pick 5 swap do
		over 3 pick negate i + bit and
		0<> if
		    4 i - bit or
		then
		
	    loop
	    
	    lcd_send
	    drop
	loop
    then

    dup 0= if 5 + then 
    
    8 0 do
	over i + c@
	\ mirror bytes 
	0
	2 pick 0 do
	    \ CR $20 emit i .
	    over 4 i - bit and
	    0<> if
		\ CR 84 emit
		2 pick negate i + 5 + bit or
	    then
	loop
	lcd_send
	drop
    loop

    5 = if
	8 0 do
	    0 lcd_send
	loop
    then

    drop
    
    0 0 lcd_pos
;

\ Monster moving pixelwise demo
\ just start after LCD_init
: monster_demo
    LCD_init
    LCD_on \ just in case we have something on stack and BL is off therefore
    $0C LCD_cmd \ disp on + cursor off
    17 0 do
	monster 0 $40 LCD_monster
	$80 i + LCD_cmd \ direct write to display memory
	0 LCD_send
	1 LCD_send
	2 LCD_send
	5 1 do
	    100 ms
	    monster i $40 LCD_monster
	loop
	$80 i + LCD_cmd
	$20 LCD_send
    loop
    \ and back ins second row 
    17 0 do
	\ direct write to display memory doesn't work in second row, why??
	16 i - 1 LCD_pos 
	0 LCD_send
	1 LCD_send
	2 LCD_send
	$20 LCD_send
	5 0 do
	    monster 4 i - $40 LCD_monster
	    100 ms
	loop
    loop
    
    \ slowly delete from char mem
    $40 lcd_cmd
    8 0 do
	0 lcd_send
	100 ms
    loop
    200 ms
    LCD_clear
    LCD_on
;

( end monster: ) here dup hex.
( size monster: ) swap - hex.
