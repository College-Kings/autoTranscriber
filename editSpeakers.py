import tkinter as tk
import json

import CK_autoTranscriber as main

class EditSpeakers(tk.Frame):

    entryList = []

    def __init__(self, parent):
        tk.Frame.__init__(self, parent)
        self.canvas = tk.Canvas(self, borderwidth=0, background="#ffffff")
        self.frame = tk.Frame(self.canvas, background="#ffffff")
        self.vsb = tk.Scrollbar(self, orient="vertical", command=self.canvas.yview)
        self.canvas.configure(yscrollcommand=self.vsb.set)

        self.vsb.pack(side="right", fill="y")
        self.canvas.pack(side="left", fill="both", expand=True)
        self.canvas.create_window((4,4), window=self.frame, anchor="nw", tags="self.frame")

        self.frame.bind("<Configure>", self.onFrameConfigure)

        self.populate()

    def populate(self):
        speakers = main.openSpeakers()
        speakersList = sorted(speakers.items())
        numRows = len(speakers) + 3

        for i in range(numRows):

            self.key = tk.Entry(self.frame, width=20, fg="blue", font=("Arial", 16, "bold"))
            self.value = tk.Entry(self.frame, width=20, fg="blue", font=("Arial", 16, "bold"))
            self.key.grid(row=i, column=0)
            self.value.grid(row=i, column=1)

            EditSpeakers.entryList.append([self.key, self.value])

            try:
                self.key.insert(0, speakersList[i][0])
                self.value.insert(0, speakersList[i][1])
            except IndexError: pass

    def onFrameConfigure(self, event):
        # Reset the scroll region to encompass the inner frame
        self.canvas.configure(scrollregion=self.canvas.bbox("all"))

    def autoUpdateSpeakers(self, parent):
        speakers = dict()
        # try:
        for key, value in EditSpeakers.entryList:
            if key.get():
                speakers[key.get()] = value.get()

        with open("speakers.json", "w") as f:
            json.dump(speakers, f, indent=4)
        parent.destroy()
        # except Exception as e: print(e)
        # finally: 

def editSpeakers():
    root = tk.Tk()
    root.title("Edit Speakers")
    root.geometry("500x500")
    editSpeakers = EditSpeakers(root)

    root.protocol('WM_DELETE_WINDOW', lambda: editSpeakers.autoUpdateSpeakers(root))
    editSpeakers.pack(side="top", fill="both", expand=True)
    root.mainloop()

if __name__ == "__main__":
    editSpeakers()