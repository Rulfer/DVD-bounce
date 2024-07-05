from tkinter import *
import time
import settings
from PIL import Image, ImageTk
from image_getter import get_image
import custom_label


class DVD:
    def __init__(self):
        # tkinter
        self.tk = Tk()

        # Get screen width and height
        self.screen_width = self.tk.winfo_screenwidth()
        self.screen_height = self.tk.winfo_screenheight()

        # Initialize Canvas
        self.canvas = Canvas(self.tk, width=self.screen_width, height=self.screen_height, bg="black")
        self.canvas.pack(anchor=CENTER, expand=True)

        self.tk.overrideredirect(True)
        self.tk.wm_attributes("-topmost", True)
        self.tk.wm_attributes("-disabled", True)
        self.tk.wm_attributes("-transparentcolor", "black")

        # Initialize time
        self.previous_time = time.time()

        # Initialize images
        self.img = get_image(self.tk)
        self.labels = []
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())

        #self.img_label = self.create_image()

        # Initialize screen position
        self.xPos = round(self.screen_width / 2)
        self.yPos = round(self.screen_height / 2)

        self.move_image()

        #self.tk.geometry(f'{self.img.width()}x{self.img.height()}+{self.xPos}+{self.yPos}')
        self.tk.geometry(f'{self.screen_width}x{self.screen_height}')
        self.tk.mainloop()

    def move_image(self):
        current_time = time.time()
        delta_time = current_time - self.previous_time
        self.previous_time = current_time

        for label in self.labels:
            label.xPos += round(settings.xVel * label.horizontal_modifier() * delta_time)
            label.yPos += round(settings.yVel * label.vertical_modifier() * delta_time)

            # Clamp value to within the screen (prevents getting stuck on edges)
            label.xPos = max(min(label.xPos, self.screen_width), 0)
            label.yPos = max(min(label.yPos, self.screen_height), 0)

            if label.xPos <= 0 or label.xPos + label.img_width >= self.screen_width:
                label.horizontal_hit()

            if label.yPos <= 0 or label.yPos + label.img_height >= self.screen_height:
                label.vertical_hit()

            label.update()

        self.tk.after(settings.milliseconds, self.move_image)

    def create_image(self):
        label = custom_label.my_label(self.tk, self.img)
        return label


DVD()
