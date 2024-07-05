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
        #self.tk.wm_attributes("-topmost", True)
        #self.tk.wm_attributes("-disabled", True)
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

            if self.hit_edge(label):
                self.hit_others(label)

            label.update()

        self.tk.after(settings.milliseconds, self.move_image)

    def create_image(self):
        label = custom_label.my_label(self.tk, self.img)
        return label

    def hit_edge(self, label):
        if label.xPos <= 0 or label.xPos + label.img_width >= self.screen_width:
            label.horizontal_hit()
            return True

        if label.yPos <= 0 or label.yPos + label.img_height >= self.screen_height:
            label.vertical_hit()
            return True

        return False

    def hit_others(self, label):
        self_pos = label.get_borders()
        my_upper_left = self_pos[0]
        my_lower_right = self_pos[1]

        for other_label in self.labels:
            if other_label is label:
                continue

            other_pos = other_label.get_borders()
            other_upper_left = other_pos[0]
            other_lower_right = other_pos[1]

            # Check if the border of the current label overlaps with any other
            # if rectangle has area 0, no overlap
            if (my_upper_left[0] == other_upper_left[0] or my_upper_left[1] == other_lower_right[1]
                    or other_lower_right[0] == my_lower_right[0] or other_lower_right[1] == my_lower_right[1]):
                return False

                # If one rectangle is on left side of other
            if my_upper_left[0] > other_lower_right[0] or my_lower_right[0] > other_upper_left[0]:
                return False

                # If one rectangle is above other
            if other_upper_left[1] > my_lower_right[1] or other_lower_right[1] > my_upper_left[1]:
                return False

            label.vertical_hit()
            label.horizontal_hit()

            return True

DVD()
