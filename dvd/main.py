from tkinter import *
import time
import settings
from PIL import Image, ImageTk
from image_getter import get_image
import animated_label


class DVD:
    def __init__(self):
        # tkinter
        self.tk = Tk()

        # Get screen width and height
        self.screen_width = self.tk.winfo_screenwidth()
        self.screen_height = self.tk.winfo_screenheight()

        # Initialize Canvas
        self.canvas = Canvas(self.tk, width=self.screen_width, height=self.screen_height, highlightthickness=0, bg="black")
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
        # What are for-loops, am I right?
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())

        self.move_image()

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

            label.update()

        self.tk.after(settings.milliseconds, self.move_image)

    def create_image(self):
        label = animated_label.MyLabel(self.tk, self.img, self.canvas)
        return label

    def get_label(self, id_filter):
        return next(x for x in self.labels if x.id == id_filter)


DVD()

