## MecrispFORTH_LCD1602

# LCD1602_direct.fs
Forth words for 4bit parallel connection to a LCD1602.

* simple Example
<pre>
    : demo
        1 LCD_init drop \ to get rid of start msg
        LCD_on \ switch backlight on
        s" Hello world!" LCD_write
        0 1 LCD_pos \ second line
        s" ============" LCD_write
    ;
</pre>

# LCD1602_i2c.fs
forth words for i2c connected LCD1602 (via PCF8574)

* usage is same as above
* uses i2c driver from JCW (link see below)

# monster.fs
demo with pixelwise moving a "10x8 sprite" over the screen (symetrie complication is just there for fun)

* works with direct and i2c connection

****

This was written on an STM32f103, but should run at least on any STM32 microcontroller.

This is written and tested for Mecrips-Stellaris Forth: http://mecrisp.sourceforge.net/

I used Jean-Claude Wippler's flib to simplify the hardware access. https://git.jeelabs.org/jcw/embello/src/branch/master/explore/1608-forth/flib/
