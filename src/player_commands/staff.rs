use bevy::prelude::*;
use leafwing_input_manager::{prelude::ToggleActions, Actionlike};
use crate::player_controller::MovementAction;

use super::{note_holder::NoteNodeHolder, notes::{ClearNotesEvent, PlayNoteAction}};

#[derive(Component, Default)]
pub struct CommandStaff
{
	pub is_open: bool,
}

// This should be enough information to map all notes
pub const F5_LINE_TOP: f32 = 15.0;
pub const STAFF_HEIGHT: f32 = 60.0;
pub const CLEF_HEIGHT: f32 = 80.0;
pub const LINE_HEIGHT: f32 = 2.0;

pub const QUARTER_NOTE_TOP_OFFSET: f32 = 41.0;
pub const QUARTER_NOTE_HEIGHT: f32 = 55.0;
pub const QUARTER_NOTE_LEFT_START: f32 = 40.0;
pub const QUARTER_NOTE_LEFT_SPACING: f32 = 20.0;

// Does top + height not actually equal bottom???
pub const QUARTER_NOTE_WEIRD_SPACING_OFFSET: f32 = 18.0;

pub fn spawn_staff(
	mut commands: Commands,
	asset_server: Res<AssetServer>,
)
{
	let treble_clef = asset_server.load("treble_clef.png");

	// Background
	commands
		.spawn((
			Name::new("Staff"),
			NodeBundle
			{
				style: Style
				{
					width: Val::Percent(100.0),
					height: Val::Px(100.0),
					flex_direction: FlexDirection::Row,
					margin: UiRect::all(Val::Px(10.0)),
					padding: UiRect::axes(Val::Px(100.0), Val::Px(10.0)),
					display: Display::None,
					..default()
				},
				background_color: bevy::color::palettes::css::BEIGE.into(),
				..default()
			},
			CommandStaff::default(),
		))
		.with_children(|parent|
		{
			// Clef
			parent
				.spawn((
					Name::new("Clef"),
					ImageBundle
					{
						image: treble_clef.into(),
						style: Style
						{
							position_type: PositionType::Absolute,
							height: Val::Px(CLEF_HEIGHT),
							..default()
						},
						..default()
					},
				));

			// Staff lines
			parent
				.spawn((
					Name::new("Staff lines"),
					NodeBundle
					{
						style: Style
						{
							flex_direction: FlexDirection::Column,
							flex_grow: 1.0,
							padding: UiRect::top(Val::Px(F5_LINE_TOP)),
							height: Val::Px(STAFF_HEIGHT),
							justify_content: JustifyContent::SpaceBetween,
							..default()
						},
						..default()
					},
					NoteNodeHolder::default(),
				))
				.with_children(|parent|
				{
					for i in 0..5
					{
						parent.spawn((
							Name::new(format!("Line {i}")),
							NodeBundle
							{
								style: Style
								{
									width: Val::Percent(100.0),
									height: Val::Px(LINE_HEIGHT),
									..default()
								},
								background_color: Color::BLACK.into(),
								..default()
							},
						));
					}
				});
		});
}

#[cfg(feature = "spawn_debug_notes_on_staff")]
pub fn spawn_debug_notes(
	mut commands: Commands,
	mut note_holder: Query<(&mut NoteNodeHolder, Entity)>,
)
{
	let (mut note_holder, note_holder_entity) = note_holder.single_mut();
	
	commands
		.entity(note_holder_entity)
		.with_children(|parent|
		{
			for note in [Note::C4, Note::D4, Note::E4, Note::F4, Note::G4, Note::A4, Note::B4, Note::C5, Note::D5, Note::E5, Note::F5, Note::G5, Note::A5]
			{
				parent.spawn(note_bundle(&mut note_holder, note.clone()));
			}
		});
}

#[derive(Actionlike, Clone, Copy, Eq, PartialEq, Hash, Reflect)]
pub enum ToggleStaffAction {
	ToggleStaff,
}

#[derive(Event, Default)]
pub struct ToggleStaffEvent;

pub fn toggle_staff(
	mut staff: Query<&mut CommandStaff>,
)
{
	let mut staff = staff.single_mut();
	staff.is_open = !staff.is_open;
}

pub fn is_staff_open(
	staff: Query<&CommandStaff>,
) -> bool
{
	staff.single().is_open
}

pub fn show_staff(
	mut staff_style: Query<&mut Style, With<CommandStaff>>,
)
{
	staff_style.single_mut().display = Display::Flex;
}

pub fn hide_staff(
	mut staff_style: Query<&mut Style, With<CommandStaff>>,
)
{
	staff_style.single_mut().display = Display::None;
}

pub fn disable_note_input(
	mut note_input: ResMut<ToggleActions<PlayNoteAction>>,
)
{
	note_input.enabled = false;
}

pub fn enable_note_input(
	mut note_input: ResMut<ToggleActions<PlayNoteAction>>,
)
{
	note_input.enabled = true;
}

pub fn disable_movement_input(
	mut movement_input: ResMut<ToggleActions<MovementAction>>,
)
{
	movement_input.enabled = false;
}

pub fn enable_movement_input(
	mut movement_input: ResMut<ToggleActions<MovementAction>>,
)
{
	movement_input.enabled = true;
}

pub fn send_clear_notes(
	mut ev_clear_notes: EventWriter<ClearNotesEvent>,
)
{
	ev_clear_notes.send(ClearNotesEvent);
}