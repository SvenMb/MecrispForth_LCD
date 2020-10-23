( start LCD: ) here dup hex.

\ don't forget to start the systicks-hz, else it will hang 

\ define Ports

\ Backlight 0 active, connected to K on LCD, but it is optionaly
PA0 constant LCD_BL
\ PA1 constant LCD_RW \ not used
PA2 constant LCD_RS \ 1 data 0 cmd
PA3 constant LCD_EN \ 1 active

\ 4 bit connection
PA4 constant LCD_D4
PA5 constant LCD_D5
PA6 constant LCD_D6
PA7 constant LCD_D7

\ 0 = cursor and blink off, 2 = cursor on , 1 = blink on , 3 = both
1 constant LCD_CB


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
    dup $80 and 0= if LCD_D7 ioc! else LCD_D7 ios! then
    dup $40 and 0= if LCD_D6 ioc! else LCD_D6 ios! then
    dup $20 and 0= if LCD_D5 ioc! else LCD_D5 ios! then
    $10 and 0= if LCD_D4 ioc! else LCD_D4 ios! then
    LCD_EN ios!
    1 ms
    LCD_EN ioc!
;


\ send byte in two nibbles, upper first
: LCD_send ( byte - - )
    dup
    LCD_sendn
    4 lshift \ move lower nibble
    LCD_sendn
    1 ms
;

: LCD_cmd
    LCD_RS ioc! \ switch to cmd mode
    LCD_send
    LCD_RS ios! \ back to data mode
    5 ms
;

: LCD_clear ( - - )
    $01 LCD_cmd
    $06 LCD_cmd \ entry mode left to right, no auto shift
;

: LCD_off
    $08 LCD_cmd
    [ifdef] LCD_BL
	LCD_BL ios!
    [then]
;

\ switches LCD on and BL on
: LCD_on
    $0C LCD_CB or LCD_cmd
    [ifdef] LCD_BL
	LCD_BL ioc!
    [then]
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

    \ initialize Ports
    [ifdef] LCD_BL
	OMODE-OD LCD_BL io-mode!
	LCD_BL ios! \ BL off
    [then]
    OMODE-OD LCD_RS io-mode!
    LCD_RS ios! \ data mode
    [ifdef] LCD_RW
	OMODE-PP LCD_RW \ read or write, not used/always low
	LCD_RW ioc!
    [then]
    OMODE-PP LCD_EN io-mode!
    LCD_EN ioc! \ nothing
    
    OMODE-PP LCD_D4 io-mode!
    OMODE-PP LCD_D5 io-mode!
    OMODE-PP LCD_D6 io-mode!
    OMODE-PP LCD_D7 io-mode!
    
    \ set cmd mode
    LCD_RS ioc! 
    
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
    $08 LCD_send \ display off
    1 ms
    \ reset to data mode reseted in LCD_on
    LCD_clear
    
    \ you don't have to show this notice
    \ comment out if you like, it is just here as demo
    \ if something is on stack don't do that and don't switch Backlight on!
    depth 0= if 
	s" LCD1602 via HD44780 on 4bit interface" LCD_write
	0 1 LCD_pos
	s" writen by SMb (c) 2020" LCD_write
	
	\ LCD and Backlight on
	LCD_on
	
	\ show and scroll a bit
	1000 ms
	21 0 do
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


( end LCD: ) here dup hex.
( size LCD: ) swap - hex.
