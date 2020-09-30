( start LCD1602: ) here dup hex.

\ driver for LCD1602 with PCF8574 i2c expander attached
\
\ don't forget to start the systicks-hz, else it will hang 

\ i2c address is configurable, standard at $27
$27 constant LCD1602.addr
\ define bits on PCF8564
$01 constant LCD_RS 
$02 constant LCD_RW
$04 constant LCD_EN
$08 constant LCD_BL
$10 constant LCD_D4
$20 constant LCD_D5
$40 constant LCD_D6
$80 constant LCD_D7

\ 0 = cursor and blink off, 2 = cursor on , 1 = blink on , 3 = both
1 constant LCD_CB

\ Backlight
0 variable LCD_BLv


\ demo char
: h
  8 lshift + 8 lshift + 8 lshift +
;

create bell
hex
04
0E
0E
0E h ,
1F
00
04
00 h ,
FFFF \ end
,
decimal



\ send only upper nibble
: LCD_sendn ( byte - - )
    LCD_EN or LCD_BLv c@ or dup LCD1602.addr i2c-addr >i2c 0 i2c-xfer drop
    1 ms
    LCD_EN bic LCD1602.addr i2c-addr >i2c 0 i2c-xfer drop    
;


\ send byte in two nibbles, upper first
: LCD_send ( byte - - )
    dup
    $F0 and LCD_RS or LCD_sendn
    4 lshift \ move lower nibble
    $F0 and LCD_RS or LCD_sendn
    1 ms
;

: LCD_cmd
    dup
    $F0 and LCD_sendn
    4 lshift \ move lower nibble
    $F0 and LCD_sendn
    5 ms
;

: LCD_clear ( - - )
    $01 LCD_cmd
    $06 LCD_cmd \ entry mode left to right, no auto shift
;

: LCD_off
    0 dup LCD_BLv c!
    Lcd_sendn
;

\ switches LCD on and BL on
: LCD_on
    LCD_BL dup LCD_BLv c!
    LCD1602.addr i2c-addr >i2c 0 i2c-xfer drop    
;

\ reset curso pos and view
: LCD_home
    $02 lcd_cmd
;

\ set cursor position 
: LCD_pos ( col row - - )
    6 lshift or $80 or
    LCD_cmd
;

\ moves display view right for positive number, left for negative numbers
: LCD_shift ( chars - - ) 
   dup 0= if exit then
   dup 0< if abs $1C else $18 then
   swap 0 do
       dup lcd_cmd
   loop
   drop
;

\ moves cursor left for positive number, right for negative numbers
: LCD_cshift ( chars - - ) 
   dup 0= if exit then
   dup 0< if abs $10 else $14 then
   swap 0 do
       dup lcd_cmd
   loop
   drop
;

\ writes a string to the display
: LCD_write ( c-addr length - - )
  0 do
    dup c@ LCD_send
    1+
  loop
  drop
;

\ load cgram or ddram for selfmade characters
\ start needs to be between 40 - 78 vor characters or 80 - for direct display
\ note current position 0 0 afterwards
: LCD_ram ( addr start - - )
    dup $40 < if 2drop exit then \ bad addr
    lcd_cmd
    begin
	dup c@
	dup $FF <>
    while
	    lcd_send
	    1+
    repeat
    2drop
    0 0 lcd_pos
;


    




\ initialize the LCD
\ writes msg in LCD and switches backlight on if nothing is on stack(!)
\ 
: LCD_init ( - - )

    \ initialize LCD
    LCD_off \ no Backlight!

    \ sequenze for 4bit 
    50 ms
    $30 LCD_sendn
    5 ms
    $30 LCD_sendn
    1 ms
    $30 LCD_sendn
    10 ms
    $20 LCD_sendn \ 4bit mode
    10 ms 
    
    \ display init
    $28 LCD_send \ 4bit mode, char size, disp size
    1 ms
    \ reset to data mode reseted in LCD_on
    LCD_clear
    LCD_on
    
    \ you don't have to show this notice
    \ comment out if you like, it is just here as demo
    \ if something is on stack don't do that and don't switch Backlight on!
    depth 0= if 
	s" LCD1602 via HD44780 on i2c interface" LCD_write
	0 1 LCD_pos
	s" writen by SMb (c) 2020" LCD_write
	
	\ LCD and Backlight on
	LCD_on
	
	\ show and scroll a bit
	1000 ms
	20 0 do
	    500 ms
	    1 lcd_shift
	loop
	1000 ms
    then

    \ cleanup again
    LCD_clear
;


\ example
\ : LCD_hello
\    s" Hello World!"
\    LCD_write
\ ;

\ example char:
\ bell $40 lcd_ram 0 lcd_send



( end LCD1602: ) here dup hex.
( size LCD1602: ) swap - hex.
