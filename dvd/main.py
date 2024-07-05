from tkinter import *
import time
import settings
from PIL import Image, ImageTk

class DVD:
    def __init__(self):
        # tkinter
        self.tk = Tk()
        self.tk.overrideredirect(True)
        self.tk.wm_attributes("-topmost", True)
        self.tk.wm_attributes("-disabled", True)
        self.tk.wm_attributes("-transparentcolor", "black")

        # Initialize time
        self.previous_time = time.time()

        # Initialize image
        self.canvas = Canvas(self.tk, bg="grey")
        self.img = PhotoImage(file="images/ODDBALL.png", master=self.tk)
        #elf.img = self.img.
        self.img_label = Label(
            master=self.tk,
            image=self.img,
            bg="black",
            width=self.img.width(),
            height=self.img.height()
        )
        print(f"Width:Height={self.img.width()}:{self.img.height()}")
        self.img_label.place(x=0, y=0)
        self.img_label.pack()

        # Get screen width and height
        self.screen_width = self.tk.winfo_screenwidth()
        self.screen_height = self.tk.winfo_screenheight()
        # Initialize screen position
        self.xPos = round(self.screen_width / 2)
        self.yPos = round(self.screen_height / 2)

        self.move_image()

        self.tk.geometry(f'{self.img.height()}x{self.img.width()}+{self.xPos}+{self.yPos}')
        self.tk.mainloop()

    def move_image(self):
        current_time = time.time()
        delta_time = current_time - self.previous_time
        self.previous_time = current_time

        self.xPos += round(settings.xVel * delta_time)
        self.yPos += round(settings.yVel * delta_time)

        if self.xPos <= 0 or self.xPos + self.img.width() >= self.screen_width:
            settings.xVel = -settings.xVel
        if self.yPos <= 0 or self.yPos + self.img.height() >= self.screen_height:
            settings.yVel = -settings.yVel  # Reverse y velocity

        self.tk.geometry(f'{self.img.width()}x{self.img.height()}+{self.xPos}+{self.yPos}')

        # Update label position
        self.img_label.place(x=0, y=0)

        self.tk.after(settings.milliseconds, self.move_image)


DVD()
