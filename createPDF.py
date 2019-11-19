import fitz

inputF = "a5.pdf"
inputI = "./idcard.png"
outF = "complete.pdf"

s=22
x=0
y=0

rect = fitz.Rect(0,0,320,180)

pdf = fitz.open(inputF)
page = pdf[0]

page.insertImage(rect, filename=inputI)

pdf.save(outF)