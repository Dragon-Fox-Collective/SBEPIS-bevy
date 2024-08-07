mod football;
mod orientation;
mod camera_controls;
mod air_movement;

use std::f32::consts::PI;
use self::football::*;
use self::orientation::*;
use self::camera_controls::*;
use self::air_movement::*;

pub use self::camera_controls::{PlayerCamera, PlayerBody, MouseSensitivity};

use bevy::prelude::*;
use bevy::render::mesh::CapsuleUvProfile;
use bevy_rapier3d::prelude::*;
use leafwing_input_manager::prelude::*;

use crate::gravity::GravityRigidbodyBundle;
use crate::gridbox_material;
use crate::input::button_input;
use crate::input::clamped_dual_axes_input;
use crate::input::dual_axes_input;
use crate::input::spawn_input_manager;

pub struct PlayerControllerPlugin;
impl Plugin for PlayerControllerPlugin
{
	fn build(&self, app: &mut App) {
		app
			.insert_resource(MouseSensitivity(0.003))
			.insert_resource(PlayerSpeed { speed: 5.0, sprint_modifier: 2.0, air_acceleration: 20.0 })
			
			.add_plugins(InputManagerPlugin::<MovementAction>::default())
			
			.add_systems(Startup, (
				setup,
				spawn_input_manager(InputMap::default()
					.insert(MovementAction::Move, VirtualDPad::wasd())
					.insert(MovementAction::Jump, KeyCode::Space)
					.insert(MovementAction::Look, DualAxis::mouse_motion())
					.insert(MovementAction::Sprint, KeyCode::ShiftLeft)
					.build()
				),
			))
			.add_systems(Update, (
				(
					orient,
					dual_axes_input(MovementAction::Look).pipe(rotate_camera_and_body),
					clamped_dual_axes_input(MovementAction::Move).pipe(axes_to_ground_velocity).pipe(spin_football),
					clamped_dual_axes_input(MovementAction::Move).pipe(axes_to_air_acceleration).pipe(air_strafe).run_if(not(is_football_on_ground)),
				).chain(),
				
				button_input(MovementAction::Jump).pipe(jump),
			))
			;
	}
}

fn setup(
	mut commands: Commands,
	mut meshes: ResMut<Assets<Mesh>>,
	mut materials: ResMut<Assets<StandardMaterial>>,
	asset_server: Res<AssetServer>,
)
{
	let position = Vec3::new(5.0, 10.0, 0.0);
	let football_local_position = Vec3::NEG_Y * 1.3;

	let body = commands.spawn((
		Name::new("Player Body"),
		PbrBundle {
			transform: Transform::from_translation(position),
			mesh: meshes.add(Capsule3d::new(0.25, 1.0).mesh().rings(1).latitudes(8).longitudes(16).uv_profile(CapsuleUvProfile::Fixed)),
			material: gridbox_material("white", &mut materials, &asset_server),
			..default()
		},
		GravityRigidbodyBundle::default(),
		Collider::capsule_y(0.5, 0.25),
		GravityOrientation,
		PlayerBody,
		LockedAxes::ROTATION_LOCKED_X | LockedAxes::ROTATION_LOCKED_Z,
	)).id();

	commands.spawn((
		Name::new("Football"),
		Football { radius: 0.5 },
		PbrBundle {
			transform: Transform::from_translation(position + football_local_position),
			mesh: meshes.add(Sphere::new(0.5).mesh().ico(2).unwrap()),
			material: gridbox_material("grey4", &mut materials, &asset_server),
			..default()
		},
		GravityRigidbodyBundle::default(),
		Collider::ball(0.5),
		Friction::new(10.0),
		FootballJoint {
			rest_local_position: football_local_position,
			jump_local_position: football_local_position + Vec3::NEG_Y * 0.25,
			jump_speed: 10.0,
		},
		ImpulseJoint::new(body, SphericalJointBuilder::new().local_anchor1(football_local_position)),
	));

	commands.spawn((
		Name::new("Football Ground Caster"),
		// RayCaster::new(football_local_position, Dir3::NEG_Y)
		// 	.with_solidness(false)
		// 	.with_max_time_of_impact(1.0)
		// 	.with_query_filter(SpatialQueryFilter::default().with_excluded_entities([body, football])),
		FootballGroundCaster,
	)).set_parent(body);

	commands.spawn((
		Name::new("Player Camera"),
		Camera3dBundle {
			transform: Transform::from_translation(Vec3::Y * 0.5),
			projection: Projection::Perspective(PerspectiveProjection {
				fov: 70.0 / 180. * PI,
				..default()
			}),
			..default()
		},
		PlayerCamera,
		Pitch(0.0),
	)).set_parent(body);
}

#[derive(Actionlike, Clone, Copy, Eq, PartialEq, Hash, Reflect)]
pub enum MovementAction
{
	Move,
	Jump,
	Look,
	Sprint,
}