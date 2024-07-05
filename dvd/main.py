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
            collisions = self.overlapping(label)
            for overlap_item, collision in collisions.items():
                l = self.get_label(collision['id'])

                if collision['right']:
                    l.go_right = False
                elif collision['left']:
                    l.go_right = True
                if collision['top']:
                    l.go_up = False
                elif collision['bottom']:
                    l.go_up = True


        self.tk.after(settings.milliseconds, self.move_image)

    def create_image(self):
        label = animated_label.MyLabel(self.tk, self.img, self.canvas)
        return label

    def overlapping(self, label):
        overlapping_items = self.canvas.find_overlapping(*self.canvas.bbox(label.id))
        collisions = {}

        bbox1 = self.canvas.bbox(label.id)
        for overlap_item in overlapping_items:
            if overlap_item != label.id:
                bbox2 = self.canvas.bbox(overlap_item)
                collisions[overlap_item] = self.determine_edges(bbox1, bbox2, overlap_item)

        return collisions

    def determine_edges(self, bbox1, bbox2, id):
        edges = {'id': id, 'left': False, 'right': False, 'top': False, 'bottom': False}

        if bbox1[2] >= bbox2[0] and bbox1[0] <= bbox2[0]:  # Right edge of bbox1 and left edge of bbox2
            edges['right'] = True
        if bbox1[0] <= bbox2[2] and bbox1[2] >= bbox2[2]:  # Left edge of bbox1 and right edge of bbox2
            edges['left'] = True
        if bbox1[3] >= bbox2[1] and bbox1[1] <= bbox2[1]:  # Bottom edge of bbox1 and top edge of bbox2
            edges['bottom'] = True
        if bbox1[1] <= bbox2[3] and bbox1[3] >= bbox2[3]:  # Top edge of bbox1 and bottom edge of bbox2
            edges['top'] = True

        return edges

    def _overlapping(self, label):
        x0, y0, x1, y1 = self.canvas.bbox(label.id)
        overlapping_items = self.canvas.find_overlapping(x0, y0, x1, y1)

        if len(overlapping_items) > 1:
            other_label = self.get_label(overlapping_items[1])

            my_coords = self.canvas.coords(label.id)
            other_coords = self.canvas.coords(other_label.id)

            # Ensure they move away horizontally
            label.go_right = True if my_coords[0] < other_coords[0] else False
            other_label.go_right = True if other_coords[0] < my_coords[0] else False

            label.go_up = True if my_coords[1] < other_coords[1] else False
            other_label.go_up = True if other_coords[1] < my_coords[1] else False

    def get_label(self, id_filter):
        return next(x for x in self.labels if x.id == id_filter)


DVD()

