from tkinter import *
import random
from PIL import Image, ImageTk
import image_getter

class MyLabel:
    def __init__(self, tk, img, c):
        self.xPos = random.randrange(start=0, stop=tk.winfo_screenwidth() - img.width())
        self.yPos = random.randrange(start=100, stop=tk.winfo_screenheight() - img.height())

        self.img = img
        self.label = Label(
            master=tk,
            image=self.img,
            bg="black",
            width=img.width(),
            height=img.height()
        )
        self.label.place(x=0, y=0)
        self.label.pack()
        self.img_width = img.width()
        self.img_height = img.height()
        self.go_right = random.choice([True, False])
        self.go_up = random.choice([True, False])
        self.id = c.create_window(self.xPos, self.yPos, window=self.label, anchor='nw')
        self.canvas = c
        self.tk = tk

    def update(self):
        self.canvas.moveto(self.id, self.xPos, self.yPos)
        self._hit_edge()

    def _hit_edge(self):
        coords = self.canvas.coords(self.id)
        if coords[0] <= 0:
            self.canvas.moveto(self.id, 0, coords[1])
            self.go_right = True
            print("GO RIGHT")
        elif coords[0] + self.img_width >= self.canvas.winfo_screenwidth():
            self.canvas.moveto(self.id, self.canvas.winfo_screenwidth() - self.img_width, coords[1])
            self.go_right = False

        if coords[1] <= 0:
            self.canvas.moveto(self.id, coords[0], 0)
            self.go_up = False
        elif coords[1] + self.img_height >= self.canvas.winfo_screenheight():
            self.canvas.moveto(self.id, coords[0], self.canvas.winfo_screenheight() - self.img_height)
            self.go_up = True

    def horizontal_modifier(self):
        return 1 if self.go_right else -1

    def vertical_modifier(self):
        return -1 if self.go_up else 1

    def set_right(self, right):
        self.go_right = right

    def set_up(self, up):
        self.go_up = up

    def vertical_hit(self):
        self.go_up = not self.go_up

    def id(self):
        return self.id()