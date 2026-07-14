from django.shortcuts import render, redirect, get_object_or_404
from django.contrib.auth.decorators import login_required
from django.http import JsonResponse
from django.views.decorators.http import require_POST
from django.contrib.auth import login
from django.contrib.auth.forms import UserCreationForm

from .models import Note
from .forms import NoteForm


@login_required(login_url='/login/')
def note_list(request):

    records = Note.objects.filter(
        user=request.user,
        is_archived=False
    ).order_by('-is_pinned', '-created_at')

    return render(request, 'Listingpage.html', {
        'records': records
    })


@login_required(login_url='/login/')
def AddNote(request):

    form = NoteForm(request.POST or None)

    if request.method == "POST":

        if form.is_valid():

            note = form.save(commit=False)
            note.user = request.user

            note.title = request.POST.get('title', '')

            note.content = request.POST.get('content', '')

            note.status = request.POST.get('status', 'draft')

            note.is_pinned = 'is_pinned' in request.POST
            note.is_archived = 'is_archived' in request.POST

            note.save()

            return redirect('note_list')

    return render(request, 'Add.html', {
        'form': form
    })

@login_required(login_url='/login/')
def EditNote(request, id):

    note = get_object_or_404(
        Note,
        pk=id,
        user=request.user
    )

    form = NoteForm(request.POST or None, instance=note)

    if request.method == "POST":

        if form.is_valid():

            note = form.save(commit=False)

            note.title = request.POST.get('title', note.title)
            note.content = request.POST.get('content', note.content)
            note.status = request.POST.get('status', note.status)

            note.is_pinned = 'is_pinned' in request.POST
            note.is_archived = 'is_archived' in request.POST

            note.save()

            return redirect('note_list')

    return render(request, 'Edit.html', {
        'form': form,
        'note': note   # ✅ THIS IS REQUIRED
    })


@require_POST
@login_required(login_url='/login/')
def autosave_note(request):

    note_id = request.POST.get('note_id')

    title = request.POST.get('title', '').strip()
    content = request.POST.get('content', '')

    status = request.POST.get('status', 'draft')

    is_pinned = 'is_pinned' in request.POST
    is_archived = 'is_archived' in request.POST

    if not title and not content:
        return JsonResponse({'status': 'ignored'})

    if note_id:

        note = get_object_or_404(
            Note,
            pk=note_id,
            user=request.user
        )

    else:

        note = Note(user=request.user)

    note.title = title or "Untitled Draft"
    note.content = content
    note.status = status
    note.is_pinned = is_pinned
    note.is_archived = is_archived

    note.save()

    return JsonResponse({
        'status': 'success',
        'note_id': note.id
    })


@login_required(login_url='/login/')
def DeleteNote(request, eid):

    note = get_object_or_404(
        Note,
        pk=eid,
        user=request.user
    )

    if request.method == "POST":
        note.delete()
        return redirect('note_list')

    return render(request, 'Delete.html', {'note': note})


@login_required(login_url='/login/')
def ViewNote(request, eid):

    note = get_object_or_404(
        Note,
        pk=eid,
        user=request.user
    )

    return render(request, 'View.html', {
        'note': note
    })


def RegisterUser(request):

    if request.method == "POST":
        form = UserCreationForm(request.POST)

        if form.is_valid():
            user = form.save()
            login(request, user)
            return redirect('note_list')

    else:
        form = UserCreationForm()

    return render(request, 'register.html', {
        'form': form
    })